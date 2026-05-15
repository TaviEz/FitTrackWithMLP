import type { DailyPlanResponseDto } from "../dtos/DailyPlanResponseDto";
import type { CreateDailyPlanDto } from "../dtos/CreateDailyPlanDto";
import type UserPhysiqueDto from "../dtos/UserPhysiqueDto";
import api from "./api";

const API_BASE_URL = 'http://localhost:8082/api/DailyPlan';

export const generateDailyPlan = async (userPhysiqueDto: UserPhysiqueDto): Promise<DailyPlanResponseDto | null> => {
    try {
        const result = await api.post<DailyPlanResponseDto>(`${API_BASE_URL}/generate`, userPhysiqueDto);
        return result.data;
    } catch (error) {
        console.log(error);
        return null;
    }
}

export const createDailyPlan = async (payload: CreateDailyPlanDto): Promise<boolean> => {
    try {
        await api.post(`${API_BASE_URL}`, payload);
        return true;
    } catch (error) {
        console.log(error);
        return false;
    }
}