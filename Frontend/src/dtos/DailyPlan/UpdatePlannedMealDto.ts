import type { UpdatePlannedIngredientDto } from "./UpdatePlannedIngredientDto";

export interface UpdatePlannedMealDto {
    title: string;
    ingredients: UpdatePlannedIngredientDto[];
}