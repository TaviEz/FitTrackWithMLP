import {
    Box,
    Card,
    CardContent,
    Chip,
    Divider,
    Stack,
    ToggleButton,
    ToggleButtonGroup,
    Typography,
} from "@mui/material";
import theme from "../../theme";
import type { MealDto } from "../../dtos/DailyPlan/MealDto";

interface GeneratedPlanPreviewProps {
    meals: MealDto[];
    targetCalories: number | null;
    actualCalories: number | null;
    mealsComplexity: string;
    onComplexityChange: (value: string) => void;
    actions: React.ReactNode;
}

const getMealCalories = (meal: MealDto) =>
    meal.ingredients.reduce(
        (sum, ingredient) => sum + ((ingredient.amountG / 100) * (ingredient.calories ?? 0)),
        0
    );

const formatCalories = (value: number) => Math.round(value);

const GeneratedPlanPreview = ({
    meals,
    targetCalories,
    actualCalories,
    mealsComplexity,
    onComplexityChange,
    actions,
}: GeneratedPlanPreviewProps) => {
    const totalCalories = meals.reduce((sum, meal) => sum + getMealCalories(meal), 0);
    const displayedActualCalories = actualCalories ?? totalCalories;

    return (
        <Box display="flex" flexDirection="column" alignItems="center" gap={3} px={2} width="100%">
            {/* Complexity toggle */}
            <Box textAlign="center" width="100%">
                <Typography sx={{ ...theme.typography.body2, mb: 1.5 }}>
                    Meal Complexity
                </Typography>
                <ToggleButtonGroup
                    value={mealsComplexity}
                    exclusive
                    onChange={(_, newComplexity) => {
                        if (newComplexity !== null) {
                            onComplexityChange(newComplexity);
                        }
                    }}
                    sx={{ border: `1px solid ${theme.palette.divider}`, borderRadius: 1 }}
                >
                    <ToggleButton value="Simple" sx={{ textTransform: "none", px: 2 }}>
                        Minimal Prep
                    </ToggleButton>
                    <ToggleButton value="Standard" sx={{ textTransform: "none", px: 2 }}>
                        Diverse Ingredients
                    </ToggleButton>
                </ToggleButtonGroup>
            </Box>

            {/* Calorie summary */}
            <Card sx={{
                width: "100%",
                maxWidth: 980,
                borderRadius: 2,
                border: `1px solid ${theme.palette.primary.main}`,
                boxShadow: "none",
                backgroundColor: theme.palette.primary.light + "14",
            }}>
                <CardContent>
                    <Stack
                        direction={{ xs: "column", sm: "row" }}
                        justifyContent="space-between"
                        spacing={3}
                        alignItems={{ xs: "flex-start", sm: "center" }}
                    >
                        <Box>
                            <Typography sx={{ ...theme.typography.body2, color: "text.secondary" }}>
                                Generated calories
                            </Typography>
                            <Typography sx={{ ...theme.typography.h4 }}>
                                {formatCalories(displayedActualCalories)} kcal
                            </Typography>
                        </Box>
                        {targetCalories !== null ? (
                            <Box>
                                <Typography sx={{ ...theme.typography.body2, color: "text.secondary" }}>
                                    Target calories
                                </Typography>
                                <Typography sx={{ ...theme.typography.h4 }}>
                                    {formatCalories(targetCalories)} kcal
                                </Typography>
                            </Box>
                        ) : (
                            <Typography sx={{ ...theme.typography.body2, color: "text.secondary" }}>
                                Target calories unavailable
                            </Typography>
                        )}
                    </Stack>
                </CardContent>
            </Card>

            {/* Meal cards */}
            <Stack spacing={2} width="100%" maxWidth={980}>
                {meals.map((meal) => (
                    <Card
                        key={meal.mealId}
                        sx={{ borderRadius: 2, border: `1px solid ${theme.palette.divider}`, boxShadow: "none" }}
                    >
                        <CardContent>
                            <Stack
                                direction={{ xs: "column", sm: "row" }}
                                justifyContent="space-between"
                                spacing={1}
                                mb={2}
                            >
                                <Box>
                                    <Typography sx={{ ...theme.typography.h5 }}>{meal.title}</Typography>
                                    <Typography sx={{ ...theme.typography.body2, color: "text.secondary" }}>
                                        {formatCalories(getMealCalories(meal))} kcal
                                    </Typography>
                                </Box>
                                <Chip
                                    label={meal.category}
                                    color="primary"
                                    variant="outlined"
                                    sx={{ alignSelf: "flex-start" }}
                                />
                            </Stack>

                            <Divider sx={{ my: 1.5 }} />

                            <Stack spacing={1}>
                                {meal.ingredients.map((ingredient) => (
                                    <Box
                                        key={`${meal.mealId}-${ingredient.foodId}`}
                                        display="flex"
                                        justifyContent="space-between"
                                        alignItems="center"
                                        gap={2}
                                    >
                                        <Typography variant="body1">{ingredient.name}</Typography>
                                        <Typography variant="body2" color="text.secondary">
                                            {ingredient.amountG} g
                                        </Typography>
                                    </Box>
                                ))}
                            </Stack>
                        </CardContent>
                    </Card>
                ))}
            </Stack>

            {/* Action buttons */}
            <Stack direction={{ xs: "column", sm: "row" }} spacing={2} mt={1}>
                {actions}
            </Stack>
        </Box>
    );
};

export default GeneratedPlanPreview;
