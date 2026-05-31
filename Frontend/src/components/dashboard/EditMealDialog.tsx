import { useState, useEffect } from "react";
import {
    Box,
    Button,
    Dialog,
    DialogContent,
    DialogTitle,
    Divider,
    IconButton,
    TextField,
    Typography,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import type { PlannedMealDto } from "../../dtos/DailyPlan/PlannedMealDto";
import type { PlannedMealIngredientDto } from "../../dtos/DailyPlan/PlannedMealIngredientDto";
import type { UpdatePlannedMealDto } from "../../dtos/DailyPlan/UpdatePlannedMealDto";
import { updatePlannedMeal } from "../../api/DailyPlanService";

interface EditMealDialogProps {
    open: boolean;
    meal: PlannedMealDto | null;
    onClose: () => void;
    onSave: () => void;
}

const EditMealDialog = ({ open, meal, onClose, onSave }: EditMealDialogProps) => {
    const [title, setTitle] = useState("");
    const [ingredients, setIngredients] = useState<PlannedMealIngredientDto[]>([]);
    const [isDirty, setIsDirty] = useState(false);

    useEffect(() => {
        if (meal) {
            setTitle(meal.title);
            setIngredients(meal.ingredients);
            setIsDirty(false);
        }
    }, [meal]);

    const computedCalories = ingredients.reduce((sum, ing) => sum + (ing.amountG / 100) * ing.calories, 0);
    const totalCalories = isDirty ? computedCalories : (meal?.totalCalories ?? 0);

    const handleAmountChange = (index: number, newAmount: number) => {
        setIngredients((prev) =>
            prev.map((ing, i) =>
                i === index ? { ...ing, amountG: newAmount } : ing
            )
        );
        setIsDirty(true);
    };

    const handleDeleteIngredient = (index: number) => {
        setIngredients((prev) => prev.filter((_, i) => i !== index));
        setIsDirty(true);
    };

    const handleAddIngredient = () => {
        const newIngredient: PlannedMealIngredientDto = {
            plannedIngredientId: 0,
            name: "New Ingredient",
            amountG: 100,
            calories: 120,
            protein: 0,
            fats: 0,
            carbs: 0
        };

        setIngredients((prev) => [...prev, newIngredient]);
        setIsDirty(true);
    };

    const handleSave = async () => {
        if (!meal) return;
        const payload: UpdatePlannedMealDto = {
            title,
            ingredients: ingredients.map((ing) => ({
                plannedIngredientId: ing.plannedIngredientId,
                amountG: ing.amountG,
            })),
        };
        const result = await updatePlannedMeal(meal.plannedMealId, payload);
        if (result.success) {
            onSave();
            onClose();
        }
    };

    if (!meal) return null;

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", pb: 1 }}>
                <Box>
                    <Typography variant="h6" fontWeight={700} component="div">
                        Edit {meal.category}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {Math.round(totalCalories)} kcal total
                    </Typography>
                </Box>
                <IconButton onClick={onClose} size="small" aria-label="close" sx={{ mt: 0.5 }}>
                    <CloseIcon />
                </IconButton>
            </DialogTitle>

            <Divider />

            <DialogContent sx={{ pt: 3, display: "flex", flexDirection: "column", gap: 3 }}>
                <TextField
                    label="Meal title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    fullWidth
                    variant="outlined"
                    size="small"
                />

                {ingredients && (
                    <Box>
                        {/* Column headers */}
                        <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 45%" }}>
                                Ingredient
                            </Typography>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 25%", textAlign: "center" }}>
                                Amount
                            </Typography>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 20%", textAlign: "center" }}>
                                Calories
                            </Typography>
                            <Box sx={{ flex: "0 0 10%", display: "flex", justifyContent: "center" }}>
                                <Button
                                    size="small"
                                    startIcon={<AddIcon />}
                                    onClick={handleAddIngredient}
                                    sx={{ 
                                        textTransform: "none", 
                                        fontSize: "0.75rem", 
                                        fontWeight: 600,
                                        whiteSpace: "nowrap",
                                        p: 0,
                                        minWidth: 0
                                    }}
                                >
                                    Add
                                </Button>
                            </Box>
                        </Box>
                        <Divider sx={{ mb: 1.5 }} />

                        {/* Ingredient rows */}
                        {ingredients.map((ing, i) => {
                            const rowCalories = Math.round((ing.amountG / 100) * ing.calories);
                            return (
                                <Box
                                    key={i}
                                    sx={{
                                        display: "flex",
                                        alignItems: "center",
                                        py: 1,
                                        borderBottom: i < ingredients.length - 1 ? "1px dashed" : "none",
                                        borderColor: "divider",
                                    }}
                                >
                                    <Box sx={{ flex: "0 0 45%", display: "flex", alignItems: "center" }}>
                                        <Typography variant="body2" fontWeight={500} noWrap>
                                            {ing.name}
                                        </Typography>
                                    </Box>
                                    <Box sx={{ flex: "0 0 25%", display: "flex", justifyContent: "center" }}>
                                        <TextField
                                            type="number"
                                            value={ing.amountG}
                                            size="small"
                                            variant="outlined"
                                            onChange={(e) => handleAmountChange(i, parseFloat(e.target.value) || 0)}
                                            slotProps={{ 
                                                input: { 
                                                    endAdornment: 
                                                    <Typography variant="caption" color="text.secondary">
                                                        g
                                                    </Typography> 
                                                    } 
                                                }}
                                            sx={{
                                                width: 90,
                                                "& input": { textAlign: "right", p: "4px 8px 4px 6px" },
                                                "& input[type=number]": { MozAppearance: "textfield" },
                                                "& input[type=number]::-webkit-outer-spin-button": { WebkitAppearance: "none", margin: 0 },
                                                "& input[type=number]::-webkit-inner-spin-button": { WebkitAppearance: "none", margin: 0 },
                                            }}
                                        />
                                    </Box>
                                    <Typography
                                        variant="body2"
                                        fontWeight={600}
                                        sx={{ flex: "0 0 20%", textAlign: "center", color: "primary.main" }}
                                    >
                                        {rowCalories} kcal
                                    </Typography>
                                    <Box sx={{ flex: "0 0 10%", display: "flex", justifyContent: "center" }}>
                                        <IconButton
                                            size="small"
                                            color="error"
                                            onClick={() => handleDeleteIngredient(i)}
                                            aria-label="delete ingredient"
                                        >
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    </Box>
                                </Box>
                            );
                        })}
                    </Box>
                )}
            </DialogContent>

            <Box sx={{ display: "flex", justifyContent: "flex-end", gap: 1, px: 3, py: 2 }}>
                <Button variant="text" onClick={onClose} sx={{ textTransform: "none" }}>
                    Cancel
                </Button>
                <Button variant="contained" onClick={handleSave} sx={{ textTransform: "none" }}>
                    Save Changes
                </Button>
            </Box>
        </Dialog>
    );
};

export default EditMealDialog;
