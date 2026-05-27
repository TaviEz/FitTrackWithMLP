import { useEffect, useState } from "react";
import {
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogContent,
    DialogTitle,
    IconButton,
    Typography,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { generateDailyPlan, createDailyPlan, replaceDailyPlan } from "../../api/DailyPlanService";
import { useUser } from "../../context/UserContext";
import UserPhysiqueDto from "../../dtos/UserDetails/UserPhysiqueDto";
import { getActivityLevelEnum } from "../../utils/types";
import type { GeneratedMealDto } from "../../dtos/DailyPlan/GeneratedMealDto";
import type { CreateDailyPlanDto } from "../../dtos/DailyPlan/CreateDailyPlanDto";
import GeneratedPlanPreview from "../shared/GeneratedPlanPreview";
import { showError } from "../shared/ShowToast";
import { SecondaryButton } from "../../styledComponents/Buttons";

interface GeneratePlanDialogProps {
    open: boolean;
    mode: "create" | "replace";
    existingPlanId?: number;
    onClose: () => void;
    onPlanAccepted: () => void;
}

const GeneratePlanDialog = ({ open, mode, existingPlanId, onClose, onPlanAccepted }: GeneratePlanDialogProps) => {
    const { userDetails } = useUser();
    const [generating, setGenerating] = useState(false);
    const [saving, setSaving] = useState(false);
    const [meals, setMeals] = useState<GeneratedMealDto[]>([]);
    const [targetCalories, setTargetCalories] = useState<number | null>(null);
    const [actualCalories, setActualCalories] = useState<number | null>(null);
    const [mealsComplexity, setMealsComplexity] = useState("Standard");

    useEffect(() => {
        if (!open) {
            setMeals([]);
            setTargetCalories(null);
            setActualCalories(null);
            setMealsComplexity("Standard");
            return;
        }
        generate("Standard");
    }, [open]); // eslint-disable-line react-hooks/exhaustive-deps

    const buildDto = (complexity: string): UserPhysiqueDto | null => {
        if (!userDetails) return null;
        const activityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
        if (!activityLevel) return null;
        return new UserPhysiqueDto(
            userDetails.tdee,
            Math.round(userDetails.weight),
            activityLevel,
            userDetails.goal,
            complexity
        );
    };

    const generate = async (complexity: string) => {
        const dto = buildDto(complexity);
        if (!dto) {
            showError("Missing user profile data. Please complete your profile first.");
            return;
        }
        setGenerating(true);
        const result = await generateDailyPlan(dto);
        setGenerating(false);
        if (result.success && result.data?.meals?.length > 0) {
            setMeals(result.data.meals);
            setTargetCalories(result.data.targetCalories);
            setActualCalories(result.data.actualCalories);
        } else {
            showError("Could not generate meals right now. Please try again.");
        }
    };

    const handleRegenerate = () => generate(mealsComplexity);

    const handleAccept = async () => {
        if (meals.length === 0) return;
        setSaving(true);
        if (mode === "replace") {
            if (!existingPlanId) {
                setSaving(false);
                showError("Could not identify the plan to replace. Please try again.");
                return;
            }
            const payload: CreateDailyPlanDto = { meals };
            console.log(payload)
            const result = await replaceDailyPlan(existingPlanId, payload);
            setSaving(false);
            if (!result.success) {
                showError("Failed to replace the plan. Please try again.");
                return;
            }
            onPlanAccepted();
            return;
        }
        const payload: CreateDailyPlanDto = { meals };
        const result = await createDailyPlan(payload);
        setSaving(false);
        if (!result.success) {
            showError(
                result.status === 409
                    ? "A daily plan already exists for today."
                    : "Failed to save the plan. Please try again."
            );
            return;
        }
        onPlanAccepted();
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
            <DialogTitle sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <Typography variant="h6" fontWeight={700} component="span">
                    Today's Meal Plan
                </Typography>
                <IconButton onClick={onClose} size="small" aria-label="close">
                    <CloseIcon />
                </IconButton>
            </DialogTitle>
            <DialogContent dividers sx={{ overflowX: "hidden", overflowY: "auto", height: 700 }}>
                {generating || meals.length === 0 ? (
                    <Box display="flex" flexDirection="column" alignItems="center" gap={2} py={6}>
                        <CircularProgress />
                        <Typography variant="body1" color="text.secondary">
                            Generating your meal plan...
                        </Typography>
                    </Box>
                ) : (
                    // TODO: rethink button names for handleAccept
                    <GeneratedPlanPreview
                        meals={meals}
                        targetCalories={targetCalories}
                        actualCalories={actualCalories}
                        mealsComplexity={mealsComplexity}
                        onComplexityChange={setMealsComplexity}
                        actions={
                            <>
                                <SecondaryButton onClick={handleRegenerate}>
                                    Regenerate Plan
                                </SecondaryButton>
                                <Button
                                    variant="contained"
                                    onClick={handleAccept}
                                    disabled={saving}
                                >
                                    {saving ? "Saving..." : mode === "replace" ? "Replace plan" : "Use this plan"}
                                </Button>
                            </>
                        }
                    />
                )}
            </DialogContent>
        </Dialog>
    );
};

export default GeneratePlanDialog;
