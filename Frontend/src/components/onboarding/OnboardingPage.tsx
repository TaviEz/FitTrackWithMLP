import { Box, Button, Card, CardContent, CircularProgress, Grid, Typography, Stepper, Step, StepLabel } from "@mui/material"
import theme from "../../theme";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import ActivityLevel from "../../models/ActivityLevel";
import UserDetails from "../../models/UserDetails";
import PersonalDetailsForm from "./PersonalDetailsForm";
import { ToastContainer } from "react-toastify";
import { showError, showInfo } from "../shared/ShowToast";
import { saveUserDetails, getUserDetails } from "../../api/UserProfileService";
import { activityLevelsData } from "../../utils/activityLevelsData";
import { useUser } from "../../context/UserContext";
import DecisionFlow from "./DecisionFlow";
import ResultsForm from "../shared/ResultsSummary";
import { createDailyPlan, generateDailyPlan } from "../../api/DailyPlanService";
import type { CreateDailyPlanDto } from "../../dtos/DailyPlan/Create/CreateDailyPlanDto";
import UserPhysiqueDto from "../../dtos/UserDetails/UserPhysiqueDto";
import { getActivityLevelEnum } from "../../utils/types";
import { SecondaryButton } from "../../styledComponents/Buttons";
import GeneratedPlanPreview from "../shared/GeneratedPlanPreview";
import type { GeneratedMealDto } from "../../dtos/DailyPlan/Generate/GeneratedMealDto";

const steps = ["Personal Details", "Choose Approach", "Meal Plan"];

const Onboarding = () => {
    const navigate = useNavigate();
    const [step, setStep] = useState<1 | 2 | 3>(1);
    const [activityLevels, setActivityLevels] = useState<ActivityLevel[]>([]);
    const [userDetails, setUserDetails] = useState<UserDetails>(UserDetails.default());
    const [generatedMeals, setGeneratedMeals] = useState<GeneratedMealDto[]>([]);
    const [userPhysiqueDto, setUserPhysiqueDto] = useState<UserPhysiqueDto | null>(null);
    const [generating, setGenerating] = useState(false);
    const [mealsComplexity, setMealsComplexity] = useState<string>("Standard");
    const [targetCalories, setTargetCalories] = useState<number | null>(null);
    const [actualCalories, setActualCalories] = useState<number | null>(null);
    const [savingPlan, setSavingPlan] = useState(false);
    const { setUserDetails: setUserDetailsContext } = useUser();

    const handleCalculateCalories = async () => {
        if (userDetails.activityLevel.multiplier !== 0) {
            const bmr = calculateBMR(userDetails);
            const tdee = Math.round(bmr * userDetails.activityLevel.multiplier);
            const updatedUserDetails = userDetails.clone();
            updatedUserDetails.bmr = bmr;
            updatedUserDetails.tdee = tdee;
            setUserDetails(updatedUserDetails);
        } else {
            showInfo("Select an activity level in order to proceed");
        }
    }

    const handleOpenActivityLevel = () => {
        if (activityLevels.length === 0) {
            setActivityLevels(activityLevelsData);
        }
    }

    const calculateBMR = (ud: UserDetails) => {
        const genderConstant = ud.gender === "female" ? -161 : 5;
        return 10 * ud.weight + 6.25 * ud.height - 5 * ud.age + genderConstant;
    }

    const handleNextSteps = async () => {
        const result = await saveUserDetails(userDetails);
        if (!result.success) {
            if (result.status === 400) {
                showError("Please check your personal details and try again.");
            } else {
                showError("Could not save your details. Please try again.");
            }
            return;
        }

        // retrieve the user details to get the computed targetCalories
        const updatedDto = await getUserDetails();
        if (updatedDto.success && updatedDto.data) {
            const updated = userDetails.clone();
            updated.targetCalories = updatedDto.data.targetCalories;
            setUserDetails(updated);
        }

        setStep(2);
    };

    const runGeneration = async (dto: UserPhysiqueDto) => {
        setGenerating(true);
        const result = await generateDailyPlan(dto);
        setGenerating(false);
        if (result.success && result.data && result.data.meals.length > 0) {
            setGeneratedMeals(result.data.meals);
            setTargetCalories(result.data.targetCalories);
            setActualCalories(result.data.actualCalories);
            setStep(3);
        } else {
            setTargetCalories(null);
            setActualCalories(null);
            console.log(result)
            if (result.errorCode === "INFEASIBLE_CONSTRAINTS") {
                showError("The optimizer could not find meals matching your nutritional targets. Try adjusting your profile to relax the parameters.");
            } else {
                showError("Could not generate meals right now. Please try again.");
            }
        }
    }

    const decisionFlowComplete = async (mode: string) => {
        if (mode === "SelfTrack") {
            setUserDetailsContext(userDetails);
            navigate("/dashboard");
            return;
        }
        if (mode === "AITrack") {
            const activityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
            if (!activityLevel) {
                showError("Please select a valid activity level before generating your plan.");
                return;
            }
            const dto = new UserPhysiqueDto(
                userDetails.tdee,
                Math.round(userDetails.weight),
                activityLevel,
                userDetails.goal,
                mealsComplexity
            );
            setUserPhysiqueDto(dto);
            await runGeneration(dto);
        }
    };

    const handleRegenerate = async () => {
        if (userPhysiqueDto) {
            // update meals complexity after regenerate
            userPhysiqueDto.mealsComplexity = mealsComplexity;
            await runGeneration(userPhysiqueDto);
        }
    };

    const saveAndNavigateToDashboard = async () => {
        if (generatedMeals.length === 0) {
            showInfo("Generate a meal plan before continuing.");
            return;
        }

        if (savingPlan) {
            return;
        }

        setSavingPlan(true);
        if (actualCalories == null) {
            showError("Actual calories not available. Please generate a plan first.");
            setSavingPlan(false);
            return;
        }
        const payload: CreateDailyPlanDto = {
            meals: generatedMeals
        };
        const dateStr = new Date().toISOString().split("T")[0];
        const result = await createDailyPlan(payload, dateStr);
        setSavingPlan(false);

        if (!result.success) {
            if (result.status === 409) {
                showError("A daily plan already exists for today.");
            } else {
                showError("Error! Failed to store the daily plan for the user.");
            }
            return;
        }
        
        setUserDetailsContext(userDetails);
        navigate("/dashboard");
    };

    return (
        <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" gap={2}>
            {/* Onboarding Stepper */}
            <Box sx={{ width: '100%', maxWidth: 600, mt: 4, mb: 2 }}>
                <Stepper activeStep={step - 1} alternativeLabel>
                    {steps.map((label) => (
                        <Step key={label}>
                            <StepLabel>{label}</StepLabel>
                        </Step>
                    ))}
                </Stepper>
            </Box>

            {/* Step 1 — User Metrics */}
            {step === 1 && (
                <>
                    <Typography sx={{ ...theme.typography.h4 }}>
                        Ready to build a healthier version of you?
                    </Typography>
                    <Typography sx={{ ...theme.typography.h5 }}>
                        Share a few details and we'll guide you every step of the way.
                    </Typography>

                    <Grid container spacing={4}>
                        <Grid size={{ xs: 12, md: 7 }}>
                            <Card sx={{ p: 3, borderRadius: 2, border: `1px solid ${theme.palette.divider}` }}>
                                <PersonalDetailsForm
                                    userDetails={userDetails}
                                    setUserDetails={setUserDetails}
                                    activityLevels={activityLevels}
                                    handleOpenActivityLevel={handleOpenActivityLevel}
                                    handleCalculateCalories={handleCalculateCalories}
                                />
                            </Card>
                        </Grid>
                        <Grid container size={{ xs: 12, md: 5 }} direction="column" alignItems="center">
                            <Card sx={{
                                minWidth: 300,
                                p: 3,
                                borderRadius: 2,
                                border: `1px solid ${theme.palette.primary.main}`,
                                backgroundColor: theme.palette.primary.light + "18"
                            }}>
                                <CardContent>
                                    <ResultsForm bmr={userDetails.bmr} tdee={userDetails.tdee} handleNextSteps={handleNextSteps} />
                                </CardContent>
                            </Card>
                        </Grid>
                    </Grid>
                </>
            )}

            {/* Step 2 — Choose Approach */}
            {step === 2 && !generating && (
                <>
                    <DecisionFlow onComplete={decisionFlowComplete} />
                    <SecondaryButton onClick={() => setStep(1)}>Back</SecondaryButton>
                </>
            )}

            {/* Loading spinner */}
            {generating && (
                <Box display="flex" flexDirection="column" alignItems="center" gap={2} mt={4}>
                    <CircularProgress />
                    <Typography variant="body1" color="text.secondary">
                        Generating your meal plan...
                    </Typography>
                </Box>
            )}

            {/* Step 3 — Meal Plan Preview */}
            {step === 3 && !generating && (
                <Box sx={{ width: "100%", maxWidth: 760 }}>
                    <GeneratedPlanPreview
                        meals={generatedMeals}
                        targetCalories={targetCalories}
                        actualCalories={actualCalories}
                        mealsComplexity={mealsComplexity}
                        onComplexityChange={setMealsComplexity}
                        actions={
                            <>
                                <SecondaryButton onClick={() => { setStep(2); setGeneratedMeals([]); }}>Back</SecondaryButton>
                                <SecondaryButton onClick={handleRegenerate}>Regenerate Plan</SecondaryButton>
                                <Button variant="contained" onClick={saveAndNavigateToDashboard} disabled={savingPlan}>
                                    {savingPlan ? "Saving..." : "Continue to Dashboard"}
                                </Button>
                            </>
                        }
                    />
                </Box>
            )}

            <ToastContainer />
        </Box>
    );
};

export default Onboarding;