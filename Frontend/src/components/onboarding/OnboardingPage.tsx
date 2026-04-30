import { Box, Card, CardContent, Grid, Typography } from "@mui/material"
import theme from "../../theme";
import { useState } from "react";
import ActivityLevel from "../../models/ActivityLevel";
import UserDetails from "../../models/UserDetails";
import PersonalDetailsForm from "./PersonalDetailsForm";
import { ToastContainer } from "react-toastify";
import { showInfo } from "../shared/ShowToast";
import { saveUserDetails } from "../../api/UserProfileService";
import { activityLevelsData } from "../../utils/activityLevelsData";
import { useUser } from "../../context/UserContext";
import DecisionFlow from "./DecisionFlow";
import ResultsForm from "../shared/ResultsSummary";

const Onboarding = () => {
    const [displayOnboardingInfo, setDisplayOnboardingInfo] = useState(true);
    const [activityLevels, setActivityLevels] = useState<ActivityLevel[]>([]);
    const [bmr, setBmr] = useState(0);
    const [tdee, setTdee] = useState(0);
    const [userDetails, setUserDetails] = useState<UserDetails>(
        UserDetails.default()
    );
    const { userId } = useUser();

    const handleCalculateCalories = async () => {        
        if (userDetails.activityLevel.multiplier !== 0) {
            const bmr = calculateBMR(userDetails);
            const tdee = bmr * userDetails.activityLevel.multiplier;
            setBmr(bmr);
            setTdee(tdee);
        }
        else {
            showInfo("Select an activity level in order to proceed");
        }
    }

    const handleOpenActivityLevel = () => {
        if (activityLevels.length == 0) {
            const data = activityLevelsData
            setActivityLevels(data);
        }
    }


    // TODO: make sure to use integer numbers for displaying info such as BMR
    const calculateBMR = (userDetails: UserDetails) => {
        const genderConstant = userDetails.gender == 'female' ? -161 : 5;
        return 10 * userDetails.weight + 6.25 * userDetails.height - 5 * userDetails.age + genderConstant;
    }

    const handleNextSteps = () => {
        console.log('next steps')
        console.log(userId)
        if (userId) {
            saveUserDetails(userDetails, userId);
            setDisplayOnboardingInfo(false);
        }
    }

    const decisionFlowComplete = (choice: string, mode: string) => {
        console.log(choice, mode);
    }

    return (
        <Box 
            display="flex" 
            flexDirection="column"
            justifyContent="center"
            alignItems="center"
            gap={3}
        >
            { displayOnboardingInfo ? (
                <>
                    <Typography
                        sx={{
                            ...theme.typography.h4
                        }} 
                    >
                        Ready to build a healthier version of you?
                    </Typography>
                    <Typography 
                        sx={{
                            ...theme.typography.h5
                        }} 
                    >
                        Share a few details and we'll guide you every step of the way.
                    </Typography>
                    
                    <Grid container spacing={4}>
                        <Grid size={{ xs: 12, md: 7 }}>
                            <Card
                                sx={{
                                    p: 3,
                                    borderRadius: 2,
                                    border: `1px solid ${theme.palette.divider}`,
                                }}
                            >
                                <PersonalDetailsForm
                                    userDetails={userDetails}
                                    setUserDetails={setUserDetails}
                                    activityLevels={activityLevels}
                                    handleOpenActivityLevel={handleOpenActivityLevel}
                                    handleCalculateCalories={handleCalculateCalories}
                                />
                            </Card>
                        </Grid>
                        <Grid
                            container
                            size={{ xs: 12, md: 5 }}
                            direction="column"
                            alignItems="center"
                        >
                            <Card
                                sx={{
                                    minWidth: 300,
                                    p: 3,
                                    borderRadius: 2,
                                    border: `1px solid ${theme.palette.primary.main}`,
                                    backgroundColor: theme.palette.primary.light + "18"
                                }}
                            >
                                <CardContent>
                                    <ResultsForm bmr={bmr} tdee={tdee} handleNextSteps={handleNextSteps}/>
                                </CardContent>
                            </Card>
                        </Grid>
                    </Grid>
                </>
            ) : (
                <DecisionFlow onComplete={decisionFlowComplete}/>
            )}

            <ToastContainer/>
        </Box>
    )
}

export default Onboarding