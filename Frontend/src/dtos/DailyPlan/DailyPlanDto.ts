import type { PlannedMealDto } from "./PlannedMealDto";

export interface DailyPlanDto {
    id: number;
    totalCalories: number;
    meals: PlannedMealDto[];
}
