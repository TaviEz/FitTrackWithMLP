import type { GeneratedDailyPlanDto } from "../dtos/DailyPlan/GeneratedDailyPlanDto";
import type { CreateDailyPlanDto } from "../dtos/DailyPlan/CreateDailyPlanDto";
import type UserPhysiqueDto from "../dtos/UserDetails/UserPhysiqueDto";
import api from "./api";
import type { DailyPlanDto } from "../dtos/DailyPlan/DailyPlanDto";

const API_BASE_URL = 'http://localhost:8082/api/DailyPlan';

export const generateDailyPlan = async (userPhysiqueDto: UserPhysiqueDto): Promise<GeneratedDailyPlanDto | null> => {
    try {
        const result = await api.post<GeneratedDailyPlanDto>(`${API_BASE_URL}/generate`, userPhysiqueDto);

        if (result.status >= 400) {
            return null;
        }

        return result.data;
    } catch (error) {
        return null;
    }
}

export const createDailyPlan = async (payload: CreateDailyPlanDto): Promise<boolean> => {
    try {
        const result = await api.post(`${API_BASE_URL}`, payload);
        return result.status < 400;
    } catch (error) {
        return false;
    }
}

export const getDailyPlan = async (dateTarget: string): Promise<DailyPlanDto | null | undefined> => {
    try {
        const result = await api.get<DailyPlanDto>(`${API_BASE_URL}`, { params: { dateTarget } });

        if (result.status === 404) {
            return null;
        }

        if (result.status >= 400) {
            return undefined;
        }

        return result.data;
    } catch (error: any) {
        return undefined; // real error
    }
}