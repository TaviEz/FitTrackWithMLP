import type { PlannedMealIngredientDto } from "./PlannedMealIngredientDto";

export interface PlannedMealDto {
    id: number;
    category: string;
    title: string;
    ingredients: PlannedMealIngredientDto[];
    totalCalories: number;
}
