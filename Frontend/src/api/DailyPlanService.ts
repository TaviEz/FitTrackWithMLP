import type { CreateDailyPlanDto } from "../dtos/DailyPlan/Create/CreateDailyPlanDto";
import type UserPhysiqueDto from "../dtos/UserDetails/UserPhysiqueDto";
import api from "./api";
import type { DailyPlanDto } from "../dtos/DailyPlan/Get/DailyPlanDto";
import type { IngredientOptionDto } from "../dtos/DailyPlan/Get/IngredientOptionDto";
import type { GeneratedDailyPlanDto } from "../dtos/DailyPlan/Generate/GeneratedDailyPlanDto";
import type { CreatePlannedIngredientDto } from "../dtos/DailyPlan/Create/CreatePlannedIngredientDto";

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

export const createDailyPlan = async (payload: CreateDailyPlanDto, targetDate: string): Promise<any> => {
    try {
        const result = await api.post(`${API_BASE_URL}`, payload, {
            params: { targetDate },
        });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const getDailyPlan = async (targetDate: string): Promise<any> => {
    try {
        const result = await api.get<DailyPlanDto>(`${API_BASE_URL}`, { params: { targetDate } });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const addPlannedMeal = async (
    dailyPlanId: number,
    payload: { category: string; title: string }
): Promise<any> => {
    try {
        const result = await api.post(`${API_BASE_URL}/${dailyPlanId}/meal`, payload);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const addPlannedIngredient = async (
    plannedMealId: number, payload: CreatePlannedIngredientDto): Promise<any> => {
    try {
        const result = await api.post(`${API_BASE_URL}/meal/${plannedMealId}/ingredient`, payload);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const updatePlannedMealTitle = async (plannedMealId: number, title: string): Promise<any> => {
    try {
        const result = await api.put(`${API_BASE_URL}/meal/${plannedMealId}/title`, { title });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const updatePlannedIngredient = async (plannedIngredientId: number, { amountG }: { amountG: number }): Promise<any> => {
    try {
        const result = await api.put(`${API_BASE_URL}/ingredient/${plannedIngredientId}`, { amountG: amountG});

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const fetchIngredientOptions = async (query: string): Promise<any> => {
    try {
        const result = await api.get<IngredientOptionDto[]>(`${API_BASE_URL}/ingredient/search`, {
            params: { query },
        });

        if (result.status >= 400) {
            return { success: false, data: [] };
        }

        return { success: true, data: result.data };
    } catch {
        return { success: false, data: [] };
    }
}

export const deletePlannedIngredient = async (plannedMealId: number, plannedIngredientId: number): Promise<any> => {
    try {
        const result = await api.delete(`${API_BASE_URL}/ingredient`, {
            params: { plannedMealId, plannedIngredientId },
        });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}
