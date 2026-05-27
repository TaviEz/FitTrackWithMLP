import type { GeneratedDailyPlanDto } from "../dtos/DailyPlan/GeneratedDailyPlanDto";
import type { CreateDailyPlanDto } from "../dtos/DailyPlan/CreateDailyPlanDto";
import type UserPhysiqueDto from "../dtos/UserDetails/UserPhysiqueDto";
import api from "./api";
import type { DailyPlanDto } from "../dtos/DailyPlan/DailyPlanDto";

const API_BASE_URL = 'http://localhost:8082/api/DailyPlan';

export const generateDailyPlan = async (userPhysiqueDto: UserPhysiqueDto): Promise<any> => {
    try {
        const result = await api.post<GeneratedDailyPlanDto>(`${API_BASE_URL}/generate`, userPhysiqueDto);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const replaceDailyPlan = async (
    dailyPlanId: number,
    payload: CreateDailyPlanDto
): Promise<any> => {
    try {
        const result = await api.put(`${API_BASE_URL}/${dailyPlanId}`, payload);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const createDailyPlan = async (payload: CreateDailyPlanDto): Promise<any> => {
    try {
        const result = await api.post(`${API_BASE_URL}`, payload);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const getDailyPlan = async (dateTarget: string): Promise<any> => {
    try {
        const result = await api.get<DailyPlanDto>(`${API_BASE_URL}`, { params: { dateTarget } });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}