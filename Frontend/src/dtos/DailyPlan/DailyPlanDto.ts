import type { PlannedMealDto } from "./PlannedMealDto";

export interface DailyPlanDto {
    totalCalories: number;
    meals: PlannedMealDto[];
}
