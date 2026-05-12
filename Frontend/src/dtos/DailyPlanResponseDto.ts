import type { MealDto } from "./MealDto";

export interface DailyPlanResponseDto {
    targetCalories: number;
    actualCalories: number;
    meals: MealDto[];
}
