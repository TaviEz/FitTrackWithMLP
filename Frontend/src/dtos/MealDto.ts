import type { MealIngredientDto } from "./MealIngredientDto";

export interface MealDto {
    category: string;
    title: string;
    meal_id: number;
    ingredients: MealIngredientDto[];
    error: number;
}