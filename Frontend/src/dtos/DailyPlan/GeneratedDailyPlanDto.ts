import type { MealDto } from "./MealDto";

export interface GeneratedDailyPlanDto {
    targetCalories: number;
    actualCalories: number;
    meals: MealDto[];
}
