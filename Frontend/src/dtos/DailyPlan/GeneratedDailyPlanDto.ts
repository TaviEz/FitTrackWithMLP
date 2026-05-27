import type { GeneratedMealDto } from "./GeneratedMealDto";

export interface GeneratedDailyPlanDto {
    targetCalories: number;
    actualCalories: number;
    meals: GeneratedMealDto[];
}
