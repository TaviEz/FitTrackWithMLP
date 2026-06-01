import { useState, useEffect } from "react";
import {
    Autocomplete,
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
import CheckIcon from "@mui/icons-material/Check";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import type { PlannedMealDto } from "../../dtos/DailyPlan/Get/PlannedMealDto";
import type { PlannedMealIngredientDto } from "../../dtos/DailyPlan/Get/PlannedMealIngredientDto";
import { addPlannedIngredient, deletePlannedIngredient, fetchIngredientOptions, updatePlannedIngredient } from "../../api/DailyPlanService";
import type { IngredientOptionDto } from "../../dtos/DailyPlan/Get/IngredientOptionDto";

interface EditMealDialogProps {
    open: boolean;
    meal: PlannedMealDto | null;
    onClose: () => void;
    onSave: () => void;
}

const EditMealDialog = ({ open, meal, onClose, onSave }: EditMealDialogProps) => {
    const [title, setTitle] = useState("");
    const [ingredients, setIngredients] = useState<PlannedMealIngredientDto[]>([]);
    const [editingIngredientId, setEditingIngredientId] = useState<number | null>(null);
    const [hasDeleted, setHasDeleted] = useState(false);
    const [hasSaved, setHasSaved] = useState(false);
    const [ingredientQuery, setIngredientQuery] = useState("");
    const [ingredientOptions, setIngredientOptions] = useState<IngredientOptionDto[]>([]);

    useEffect(() => {
        if (!ingredientQuery.trim()) {
            setIngredientOptions([]);
            return;
        }

        // debounce the api call for 300ms
        const timer = setTimeout(async () => {
            const results = await fetchIngredientOptions(ingredientQuery);
            setIngredientOptions(results);
        }, 300);
        return () => clearTimeout(timer);
    }, [ingredientQuery]);

    useEffect(() => {
        if (meal) {
            setTitle(meal.title);
            setIngredients(meal.ingredients);
            setEditingIngredientId(null);
            setHasDeleted(false);
            setHasSaved(false);
        }
    }, [meal]);

    const totalCalories = ingredients.reduce((sum, ing) => sum + (ing.amountG / 100) * ing.calories, 0);

    const handleAmountChange = (index: number, newAmount: number) => {
        const ingredientId = ingredients[index].plannedIngredientId;

        if (editingIngredientId !== null && editingIngredientId !== ingredientId) return;

        setIngredients((prev) =>
            prev.map((ing, i) =>
                i === index ? { ...ing, amountG: newAmount } : ing
            )
        );
        setEditingIngredientId(ingredientId);
    };

    const handleDeleteIngredient = async (index: number) => {
        const ingredient = ingredients[index];

        if (ingredient.plannedIngredientId !== 0) {
            const result = await deletePlannedIngredient(meal!.plannedMealId, ingredient.plannedIngredientId);
            if (!result.success) return;
        }

        setIngredients((prev) => prev.filter((_, i) => i !== index));
        setHasDeleted(true);
    };

    const handleAddIngredient = () => {
        const newIngredient: PlannedMealIngredientDto = {
            plannedIngredientId: 0,
            name: "",
            amountG: 0,
            calories: 0,
            protein: 0,
            fats: 0,
            carbs: 0
        };
        
        // add on the ui an ingredient with some default values
        setIngredients((prev) => [...prev, newIngredient]);
        // set the id to 0
        // in the conditional rendering it knows to display the autocomplete
        setEditingIngredientId(0);
    };

    const handleIngredientSelect = (index: number, selected: IngredientOptionDto | null) => {
        if (!selected) return;

        // we keep the previous properties of the ingredient: plannedIngredientId and amountG
        // and add the properties of ingredientOptionsDto using the spread operator
        setIngredients((prev) =>
            prev.map((ing, i) =>
                i === index
                    ? { ...ing,
                        foodId: selected.food_id,
                        name: selected.name,
                        calories: selected.calories,
                        protein: selected.protein,
                        fats: selected.fat,
                        carbs: selected.carbs
                    } : ing
            )
        );
    };

    const handleClose = () => {
        if (hasDeleted || hasSaved) {
            onSave();
        } else {
            onClose();
        }
    };

    const handleSave = async () => {
        if (!meal) return;

        // if the editing element has id = 0 => we are adding an ingredient
        // otherwise we are updating an ingredient
        if (editingIngredientId === 0) {
            const newIngredient = ingredients.find((ing) => ing.plannedIngredientId === 0);
            if (!newIngredient?.foodId) return;
            
            const { plannedIngredientId, ...addIngredientDto } = newIngredient as Required<PlannedMealIngredientDto>;
            await addPlannedIngredient(meal.plannedMealId, addIngredientDto);

            setEditingIngredientId(null);
            setHasSaved(true);
            return;
        }

        const editingIngredient = ingredients.find((ing) => ing.plannedIngredientId === editingIngredientId);
        if (!editingIngredient) return;

        await updatePlannedIngredient(editingIngredient.plannedIngredientId, { amountG: editingIngredient.amountG });

        setEditingIngredientId(null);
        setHasSaved(true);
    };

    const handleSaveTitle = async () => {
        if (!meal) return;

        // TODO: call title update API
        // await updatePlannedMealTitle(meal.plannedMealId, { title });

        setHasSaved(true);
    };

    if (!meal) return null;

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
            <DialogTitle sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", pb: 1 }}>
                <Box>
                    <Typography variant="h6" fontWeight={700} component="div">
                        Edit {meal.category}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {Math.round(totalCalories)} kcal total
                    </Typography>
                </Box>
                <IconButton onClick={handleClose} size="small" aria-label="close" sx={{ mt: 0.5 }}>
                    <CloseIcon />
                </IconButton>
            </DialogTitle>

            <Divider />

            <DialogContent sx={{ pt: 3, display: "flex", flexDirection: "column", gap: 3 }}>
                <TextField
                    label="Meal title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    onBlur={handleSaveTitle}
                    fullWidth
                    variant="outlined"
                    size="small"
                />

                {ingredients && (
                    <Box>
                        {/* Column headers */}
                        <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 35%" }}>
                                Ingredient
                            </Typography>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 25%", textAlign: "center" }}>
                                Amount
                            </Typography>
                            <Typography variant="caption" color="text.secondary" sx={{ flex: "0 0 20%", textAlign: "center" }}>
                                Calories
                            </Typography>
                            <Box sx={{ flex: "0 0 20%", display: "flex", justifyContent: "center" }}>
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
                            const isThisRowEditing = editingIngredientId == ing.plannedIngredientId;
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
                                    <Box sx={{ flex: "0 0 35%", display: "flex", alignItems: "center" }}>
                                        {ing.plannedIngredientId === 0 ? (
                                            <Autocomplete
                                                options={ingredientOptions}
                                                getOptionLabel={(opt) => opt.name}
                                                size="small"
                                                fullWidth
                                                onInputChange={(_, value) => setIngredientQuery(value)}
                                                onChange={(_, selected) => handleIngredientSelect(i, selected)}
                                                renderInput={(params) => (
                                                    <TextField {...params} placeholder="Search ingredient" variant="outlined" />
                                                )}
                                                sx={{
                                                    "& .MuiInputBase-root": { py: "1px", fontSize: "0.8125rem" },
                                                    "& .MuiInputBase-input": { py: "2px !important" },
                                                }}
                                            />
                                        ) : (
                                            <Typography variant="body2" fontWeight={500} noWrap>
                                                {ing.name}
                                            </Typography>
                                        )}
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
                                    <Box sx={{ flex: "0 0 20%", display: "flex", justifyContent: "center", gap: 0.5 }}>
                                        {isThisRowEditing && (
                                            <IconButton
                                                size="small"
                                                color="success"
                                                onClick={handleSave}
                                                aria-label="save changes"
                                            >
                                                <CheckIcon fontSize="small" />
                                            </IconButton>
                                        )}
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

            <Box sx={{ display: "flex", justifyContent: "flex-end", px: 3, py: 2 }}>
                <Button variant="contained" onClick={handleClose} sx={{ textTransform: "none" }}>
                    Done
                </Button>
            </Box>
        </Dialog>
    );
};

export default EditMealDialog;
