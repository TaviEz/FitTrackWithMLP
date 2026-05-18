import type { PlannedMealIngredientDto } from "./PlannedMealIngredientDto";

export interface PlannedMealDto {
    category: string;
    title: string;
    ingredients: PlannedMealIngredientDto[];
    totalCalories: number;
}
