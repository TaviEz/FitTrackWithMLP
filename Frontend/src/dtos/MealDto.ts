import type { MealIngredientDto } from "./MealIngredientDto";

export interface MealDto {
    category: string;
    title: string;
    mealId: number;
    ingredients: MealIngredientDto[];
    error: number;
}