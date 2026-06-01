import type { GeneratedMealIngredientDto } from "./GeneratedMealIngredientDto";

export interface GeneratedMealDto {
    category: string;
    title: string;
    mealId: number;
    ingredients: GeneratedMealIngredientDto[];
    error: number;
}