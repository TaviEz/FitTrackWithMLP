import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import {
    Box,
    Typography,
    Button,
    Card,
    CardContent,
    CircularProgress,
    IconButton,
    LinearProgress,
    Divider,
    Stack,
} from "@mui/material";
import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import EditIcon from "@mui/icons-material/Edit";
import RestaurantMenuIcon from "@mui/icons-material/RestaurantMenu";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import AddIcon from "@mui/icons-material/Add";
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { useUser } from "../../context/UserContext";
import { getDailyPlan, createDailyPlan } from "../../api/DailyPlanService";
import { showError } from "../shared/ShowToast";
import { ToastContainer } from "react-toastify";
import type { DailyPlanDto } from "../../dtos/DailyPlan/Get/DailyPlanDto";
import type { PlannedMealDto } from "../../dtos/DailyPlan/Get/PlannedMealDto";
import GeneratePlanDialog from "./GeneratePlanDialog";
import EditMealDialog from "./EditMealDialog";

const CATEGORIES = ["Breakfast", "Lunch", "Snack", "Dinner"];
const NAV_HEIGHT = 64;

const Dashboard = () => {
    const { userId, userDetails } = useUser();
    const location = useLocation();
    const [dailyPlan, setDailyPlan] = useState<DailyPlanDto | null>(null);
    const [targetCalories, setTargetCalories] = useState<number>(0);
    const [loading, setLoading] = useState(true);
    const [generatePlanOpen, setGeneratePlanOpen] = useState(false);
    const [selectedDate, setSelectedDate] = useState<Date>(new Date());
    const [editMeal, setEditMeal] = useState<PlannedMealDto | null>(null);

    const fetchDailyPlan = async (date: Date) => {
        setLoading(true);
        const dateStr = date.toISOString().split("T")[0];
        const result = await getDailyPlan(dateStr);
        if (!result.success) {
            showError("Failed to load daily plan");
        } else {
            setDailyPlan(result.data);
        }
        setLoading(false);
    };

    useEffect(() => {
        if (!userId) return;
        fetchDailyPlan(selectedDate);
    }, [userId, location.key, selectedDate]); // eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        setTargetCalories(userDetails?.targetCalories ?? 0);
    }, [userDetails?.targetCalories]);

    const handleAddManually = async () => {
        const dateStr = selectedDate.toISOString().split("T")[0];
        const result = await createDailyPlan({ meals: [] }, dateStr);
        if (!result.success) {
            showError("Failed to create plan");
        } else {
            await fetchDailyPlan(selectedDate);
        }
    };

    const handleGenerateWithAI = () => {
        setGeneratePlanOpen(true);
    };

    const handlePlanAccepted = async () => {
        setGeneratePlanOpen(false);
        await fetchDailyPlan(selectedDate);
    };

    const handleEditMeal = (meal: PlannedMealDto) => {
        setEditMeal(meal);
    };

    const mealsByCategory = (category: string): PlannedMealDto[] =>
        dailyPlan?.meals.filter(
            (m) => m.category.toLowerCase() === category.toLowerCase()
        ) ?? [];


    // TODO: check why the daily calories/target calories do not match the calories in the
    // generate/regenerate plan modal
    const caloriePercent = targetCalories > 0
        ? Math.min((dailyPlan?.totalCalories ?? 0) / targetCalories * 100, 100)
        : 0;

    if (loading) {
        return (
            <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                sx={{ mt: `${NAV_HEIGHT}px`, height: `calc(100vh - ${NAV_HEIGHT}px)` }}
            >
                <CircularProgress />
            </Box>
        );
    }

    return (
        <>
            <Box
                sx={{
                    mt: `${NAV_HEIGHT}px`,
                    height: `calc(100vh - ${NAV_HEIGHT}px)`,
                    width: "100%",
                    display: "flex",
                    flexDirection: "column",
                    overflow: "hidden",
                }}
            >
                {/* ── Header bar ── */}
                <Box
                    sx={{
                        display: "flex",
                        alignItems: "center",
                        gap: 3,
                        px: 3,
                        py: 1.5,
                        borderBottom: 1,
                        borderColor: "divider",
                        flexShrink: 0,
                    }}
                >
                    <Typography variant="h6" fontWeight={700} sx={{ whiteSpace: "nowrap" }}>
                        Daily Plan
                    </Typography>

                    {dailyPlan && (
                        <Box sx={{ flex: 1, maxWidth: 320 }}>
                            <Box display="flex" justifyContent="space-between" mb={0.5}>
                                <Typography variant="caption" color="text.secondary">Calories</Typography>
                                <Typography variant="caption" color="text.secondary">
                                    {dailyPlan.totalCalories} / {targetCalories} kcal
                                </Typography>
                            </Box>
                            <LinearProgress
                                variant="determinate"
                                value={caloriePercent}
                                sx={{ height: 6, borderRadius: 3 }}
                            />
                        </Box>
                    )}

                    <Box sx={{ flex: 1 }} />
                    <Stack direction="row" spacing={1}>
                        {/* display add manually only if the user does not have a daily plan */}
                        {!dailyPlan && (
                            <Button
                                variant="outlined"
                                size="small"
                                onClick={handleAddManually}
                                sx={{ textTransform: "none" }}
                            >
                                Add manually
                            </Button>
                        )}
                        <Button
                            variant="contained"
                            size="small"
                            startIcon={<AutoAwesomeIcon />}
                            onClick={handleGenerateWithAI}
                            sx={{ textTransform: "none" }}
                        >
                            {dailyPlan ? "Regenerate" : "Generate with AI"}
                        </Button>
                    </Stack>
                </Box>
                {/* Calendar display and logic */}
                <Box
                    sx={{
                        px: 3,
                        py: 1.5,
                        flexShrink: 0,
                        borderBottom: 1,
                        borderColor: "divider",
                        display: "flex",
                        alignItems: "center",
                        backgroundColor: "background.paper",
                    }}
                >
                    {/* Left: flex:1 so it claims equal space as the right side */}
                    <Box sx={{ flex: 1 }}>
                        <DatePicker
                            label="Pick Date"
                            value={selectedDate}
                            onChange={(newDate) => {
                                if (newDate) setSelectedDate(newDate);
                            }}
                            slotProps={{
                                textField: {
                                    size: 'small',
                                    variant: 'standard'
                                }
                            }}
                        />
                    </Box>

                    {/* Center: selected date*/}
                    <Stack 
                        direction="row" 
                        spacing={2} 
                        alignItems="center"
                    >
                        <IconButton 
                            size="small" 
                            onClick={() => {
                                const nextDate = new Date(selectedDate);
                                nextDate.setDate(selectedDate.getDate() - 1);
                                setSelectedDate(nextDate);
                            }}
                        >
                            <ChevronLeftIcon />
                        </IconButton>
                        
                        <Typography 
                            variant="body1" 
                            fontWeight={700} 
                            sx={{ 
                                minWidth: 180,
                                textAlign: "center", 
                                fontSize: "1.05rem",
                                color: "text.primary" 
                            }}
                        >
                            {selectedDate.toLocaleDateString("en-US", { weekday: "long", month: "short", day: "numeric" })}
                        </Typography>

                        <IconButton 
                            size="small"
                            onClick={() => {
                                const nextDate = new Date(selectedDate);
                                nextDate.setDate(selectedDate.getDate() + 1);
                                setSelectedDate(nextDate);
                            }}
                        >
                            <ChevronRightIcon />
                        </IconButton>
                    </Stack>

                    {/* Right: flex:1 mirrors the left, button pushed to the edge */}
                    <Box sx={{ flex: 1, display: "flex", justifyContent: "flex-end" }}>
                        <Button 
                            variant="text" 
                            size="small" 
                            sx={{ textTransform: "none", fontWeight: 600 }}
                            onClick={() => setSelectedDate(new Date())}
                        >
                            Today
                        </Button>
                    </Box>
                </Box>
                {/* ── Content ── */}
                {dailyPlan ? (
                    <Box sx={{ display: "flex", flexDirection: "column", flex: 1, minHeight: 0, overflow: "hidden" }}>
                        <Box 
                            sx={{ 
                                display: "flex", 
                                flexDirection: "row",
                                justifyContent: "center",
                                width: "100%",
                                maxWidth: "1400px",
                                minHeight: 0, 
                                gap: 3, 
                                px: 4, 
                                pt: 6,
                                pb: 4,
                                alignItems: "flex-start",
                                boxSizing: "border-box"
                            }}
                        >
                            {CATEGORIES.map((category) => {
                                const meal = mealsByCategory(category)[0] ?? null;
                                return (
                                    <Box
                                        key={category}
                                        sx={{
                                            flex: 1,
                                            minWidth: "240px",  
                                            display: "flex",
                                            flexDirection: "column",
                                        }}
                                    >
                                        <Typography 
                                            variant="subtitle2" 
                                            fontWeight={700} 
                                            color="text.secondary" 
                                            mb={1.5} 
                                            sx={{ textTransform: "uppercase", letterSpacing: 1 }}
                                        >
                                            {category}
                                        </Typography>

                                        <Card 
                                            variant="outlined" 
                                            sx={{ 
                                                flex: 1, 
                                                display: "flex", 
                                                flexDirection: "column", 
                                                minHeight: 0,
                                                borderRadius: 3,
                                                boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.03)",
                                                backgroundColor: "background.paper",
                                                border: "1px solid",
                                                borderColor: "divider"
                                            }}
                                        >
                                            <CardContent sx={{ p: 3, flex: 1, display: "flex", flexDirection: "column", "&:last-child": { pb: 3 } }}>
                                                {meal && (
                                                    <>
                                                        <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={0.5}>
                                                            <Box flex={1}>
                                                                <Typography variant="subtitle1" fontWeight={700} lineHeight={1.3} mb={0.5}>
                                                                    {meal.title}
                                                                </Typography>
                                                                <Typography variant="body2" fontWeight={600} color="primary.main">
                                                                    {meal.totalCalories} kcal
                                                                </Typography>
                                                            </Box>
                                                            <IconButton
                                                                size="small"
                                                                onClick={() => handleEditMeal(meal)}
                                                                aria-label="edit meal"
                                                                sx={{ ml: 0.5, mt: -0.5, flexShrink: 0 }}
                                                            >
                                                                <EditIcon fontSize="small" />
                                                            </IconButton>
                                                        </Box>
                                                        <Divider sx={{ my: 2 }} />
                                                    </>
                                                )}

                                                {meal && meal.ingredients && meal.ingredients.length > 0 ? (
                                                    <Box sx={{ flex: 1, overflowY: "auto" }}>
                                                    {meal.ingredients.map((ing, i) => (
                                                        <Box
                                                            key={i}
                                                            sx={{
                                                                display: "flex",
                                                                justifyContent: "space-between",
                                                                alignItems: "center",
                                                                py: 1,
                                                                borderBottom: i < meal.ingredients.length - 1 ? "1px dashed" : "none",
                                                                borderColor: "divider",
                                                            }}
                                                        >
                                                            <Typography variant="body2" color="text.primary" sx={{ fontWeight: 500 }}>
                                                                {ing.name}
                                                            </Typography>
                                                            <Typography variant="body2" color="text.secondary" sx={{ ml: 1, fontWeight: 600 }}>
                                                                {ing.amountG}g
                                                            </Typography>
                                                        </Box>
                                                    ))}
                                                </Box>
                                                ) : ( // meal is null or has no ingredients — guide user to add
                                                    <Box
                                                        onClick={() => {
                                                            if (meal) {
                                                                handleEditMeal(meal);
                                                            } else {
                                                                handleEditMeal({
                                                                    plannedMealId: 0,
                                                                    category: category,
                                                                    title: `${category} Meal`,
                                                                    ingredients: [],
                                                                    totalCalories: 0
                                                                });
                                                            }
                                                        }}
                                                        sx={{
                                                            flex: 1,
                                                            display: "flex",
                                                            flexDirection: "column",
                                                            alignItems: "center",
                                                            justifyContent: "center",
                                                            cursor: "pointer",
                                                            color: "text.disabled",
                                                            py: 4,
                                                            transition: "color 0.2s",
                                                            "&:hover": {
                                                                color: "primary.main",
                                                            },
                                                        }}
                                                    >
                                                        <AddIcon sx={{ mb: 0.5 }} />
                                                        <Typography variant="caption" fontWeight={600}>
                                                            Log your {category.toLowerCase()}
                                                        </Typography>
                                                    </Box>
                                                )}
                                            </CardContent>
                                        </Card>
                                    </Box>
                                );
                            })}
                    </Box>
                    </Box>
                ) : (
                    <Box
                        display="flex"
                        flexDirection="column"
                        alignItems="center"
                        justifyContent="center"
                        textAlign="center"
                        mt={20}
                        p={5}
                        sx={{
                            borderRadius: 4,
                            border: "2px dashed",
                            borderColor: "divider",
                            bgcolor: "background.paper",
                        }}
                    >
                        <RestaurantMenuIcon sx={{ fontSize: 72, color: "primary.light", mb: 2, opacity: 0.7 }} />
                        <Typography variant="h5" fontWeight={700} mb={1}>
                            No plan for today
                        </Typography>
                        <Typography variant="body2" color="text.secondary" maxWidth={340}>
                            You haven't logged any meals yet. Let AI build a plan for you.
                        </Typography>
                    </Box>
                )}
            </Box>

            <GeneratePlanDialog
                open={generatePlanOpen}
                mode={dailyPlan ? "replace" : "create"}
                existingPlanId={dailyPlan?.dailyPlanId}
                dateTarget={selectedDate.toISOString().split("T")[0]}
                onClose={() => setGeneratePlanOpen(false)}
                onPlanAccepted={handlePlanAccepted}
            />
            <EditMealDialog
                open={editMeal !== null}
                meal={editMeal}
                onClose={() => setEditMeal(null)}
                onSave={async () => {
                    setEditMeal(null);
                    await fetchDailyPlan(selectedDate);
                }}
            />
            <ToastContainer />
        </>
    );
};

export default Dashboard;
