export interface PlannedMealIngredientDto {
    plannedIngredientId: number;
    foodId?: number;
    name: string;
    amountG: number;
    protein: number;
    fats: number;
    carbs: number;
    calories: number;
}
