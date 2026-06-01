import type { PlannedMealDto } from "./PlannedMealDto";

export interface DailyPlanDto {
    dailyPlanId: number;
    totalCalories: number;
    meals: PlannedMealDto[];
}
