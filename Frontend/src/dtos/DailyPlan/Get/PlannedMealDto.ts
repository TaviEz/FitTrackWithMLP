import type { PlannedMealIngredientDto } from "./PlannedMealIngredientDto";

export interface PlannedMealDto {
    plannedMealId: number;
    category: string;
    title: string;
    ingredients: PlannedMealIngredientDto[];
    totalCalories: number;
}
