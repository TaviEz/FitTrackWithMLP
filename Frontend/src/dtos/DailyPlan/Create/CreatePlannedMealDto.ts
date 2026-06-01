import type { CreatePlannedIngredientDto } from "./CreatePlannedIngredientDto";

export interface CreatePlannedMealDto {
    category: string;
    title: string;
    mealId: number;
    ingredients: CreatePlannedIngredientDto[];
    error: number;
}