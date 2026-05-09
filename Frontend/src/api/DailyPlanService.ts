import type { MealDto } from "../dtos/MealDto";
import type UserPhysiqueDto from "../dtos/UserPhysiqueDto";
import api from "./api";

const API_BASE_URL = 'http://localhost:8082/api/DailyPlan';

export const generateDailyPlan = async (userPhysiqueDto: UserPhysiqueDto): Promise<MealDto[]> => {
    try {
        const result = await api.post<MealDto | MealDto[]>(`${API_BASE_URL}/generate`, userPhysiqueDto);
        return Array.isArray(result.data) ? result.data : [result.data];
    } catch (error) {
        console.log(error);
        return [];
    }
}