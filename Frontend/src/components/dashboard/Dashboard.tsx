import { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Button,
    Card,
    CardContent,
    CircularProgress,
    LinearProgress,
    Divider,
    Stack,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import RestaurantMenuIcon from "@mui/icons-material/RestaurantMenu";
import { useUser } from "../../context/UserContext";
import { getDailyPlan } from "../../api/DailyPlanService";
import { showError } from "../shared/ShowToast";
import { ToastContainer } from "react-toastify";
import type { DailyPlanDto } from "../../dtos/DailyPlan/DailyPlanDto";
import type { PlannedMealDto } from "../../dtos/DailyPlan/PlannedMealDto";
import GeneratePlanDialog from "./GeneratePlanDialog";

const CATEGORIES = ["Breakfast", "Lunch", "Snack", "Dinner"];

const Dashboard = () => {
    const { userId, userDetails } = useUser();
    const [dailyPlan, setDailyPlan] = useState<DailyPlanDto | null>(null);
    const [targetCalories, setTargetCalories] = useState<number>(0);
    const [loading, setLoading] = useState(true);
    const [generatePlanOpen, setGeneratePlanOpen] = useState(false);

    const fetchDailyPlan = async () => {
        setLoading(true);
        const today = new Date().toISOString().split("T")[0];
        const result = await getDailyPlan(today);
        if (!result.success) {
            showError("Failed to load daily plan");
        } else {
            setDailyPlan(result.data);
        }
        setLoading(false);
    };

    useEffect(() => {
        if (!userId) return;
        fetchDailyPlan();
    }, [userId]); // eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        setTargetCalories(userDetails?.targetCalories ?? 0);
    }, [userDetails?.targetCalories]);

    const handleAddMeal = () => {
        // TODO: open add meal dialog / navigate
    };

    const handleGenerateWithAI = () => {
        setGeneratePlanOpen(true);
    };

    const handlePlanAccepted = async () => {
        setGeneratePlanOpen(false);
        await fetchDailyPlan();
    };

    const mealsByCategory = (category: string): PlannedMealDto[] =>
        dailyPlan?.meals.filter(
            (m) => m.category.toLowerCase() === category.toLowerCase()
        ) ?? [];

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <>
            {/* make sure you dont have a scrollbar on this page */}
            <Box sx={{ p: 3, pt: 8, maxWidth: 800, mx: "auto" }}>
            {/* Header */}
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Typography variant="h5" fontWeight={700}>
                    Daily Plan
                </Typography>
                {dailyPlan && (
                    <Button
                        variant="contained"
                        startIcon={<AutoAwesomeIcon />}
                        onClick={handleGenerateWithAI}
                        sx={{ textTransform: "none" }}
                    >
                        Regenerate Plan
                    </Button>
                )}
            </Box>

            {/* Calorie summary */}
            {dailyPlan && (
                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Box display="flex" justifyContent="space-between" mb={1}>
                            <Typography variant="subtitle1" fontWeight={600}>
                                Calories
                            </Typography>
                            <Typography variant="subtitle1" fontWeight={600}>
                                {dailyPlan.totalCalories} / {targetCalories} kcal
                            </Typography>
                        </Box>
                        <LinearProgress
                            variant="determinate"
                            value={Math.min((dailyPlan.totalCalories / targetCalories) * 100, 100)}
                            sx={{ height: 8, borderRadius: 4 }}
                        />
                    </CardContent>
                </Card>
            )}

            {/* Meals by category */}
            {dailyPlan ? (
                CATEGORIES.map((category) => {
                    const meals = mealsByCategory(category);
                    return (
                        <Box key={category} mb={4}>
                            <Typography variant="h6" fontWeight={700} mb={1}>
                                {category}
                            </Typography>
                            <Divider sx={{ mb: 2 }} />
                            {meals.length > 0 ? (
                                meals.map((meal) => (
                                    <Card key={meal.title} sx={{ mb: 2 }}>
                                        <CardContent>
                                            <Typography
                                                variant="overline"
                                                color="primary"
                                                fontWeight={700}
                                                display="block"
                                                lineHeight={1.5}
                                            >
                                                {meal.category}
                                            </Typography>
                                            <Typography variant="body1" fontWeight={600}>
                                                {meal.title}
                                            </Typography>
                                        </CardContent>
                                    </Card>
                                ))
                            ) : (
                                <Typography color="text.secondary" variant="body2">
                                    No {category.toLowerCase()} added
                                </Typography>
                            )}
                        </Box>
                    );
                })
            ) : (
                // Display friendly message in case there are no meals
                <Box
                    display="flex"
                    flexDirection="column"
                    alignItems="center"
                    justifyContent="center"
                    textAlign="center"
                    mt={6}
                    p={5}
                    sx={{
                        borderRadius: 4,
                        border: "2px dashed",
                        borderColor: "divider",
                        bgcolor: "background.paper",
                    }}
                >
                    <RestaurantMenuIcon
                        sx={{ fontSize: 72, color: "primary.light", mb: 2, opacity: 0.7 }}
                    />
                    <Typography variant="h5" fontWeight={700} mb={1}>
                        No plan for today
                    </Typography>
                    <Typography variant="body2" color="text.secondary" mb={3} maxWidth={340}>
                        You haven't logged any meals yet. Let AI build a plan for you, or add meals manually.
                    </Typography>
                    <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                        <Button
                            variant="contained"
                            size="large"
                            startIcon={<AutoAwesomeIcon />}
                            onClick={handleGenerateWithAI}
                            sx={{ textTransform: "none", px: 4, borderRadius: 3 }}
                        >
                            Generate with AI
                        </Button>
                        <Button
                            variant="outlined"
                            size="large"
                            startIcon={<AddIcon />}
                            onClick={handleAddMeal}
                            sx={{ textTransform: "none", px: 4, borderRadius: 3 }}
                        >
                            Add manually
                        </Button>
                    </Stack>
                </Box>
            )}
        </Box>
        <GeneratePlanDialog
            open={generatePlanOpen}
            mode={dailyPlan ? "replace" : "create"}
            existingPlanId={dailyPlan?.dailyPlanId}
            onClose={() => setGeneratePlanOpen(false)}
            onPlanAccepted={handlePlanAccepted}
        />
        <ToastContainer />
        </>
    );
};

export default Dashboard;
