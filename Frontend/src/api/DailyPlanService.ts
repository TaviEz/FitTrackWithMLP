import UserDetailsDto from "../dtos/UserDetailsDto";
import type UserDetails from "../models/UserDetails";
import { getActivityLevelEnum } from "../utils/types";
import api from "./api";

const API_BASE_URL = 'http://localhost:8082/api/DailyPlan';

export const generateDailyPlan = async (userDetails: UserDetails): Promise<any> => {
    try {
        const userActivityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
        const userDto = UserDetailsDto.fromDomain(userDetails, userActivityLevel);
        const result = await api.post(`${API_BASE_URL}/generate`, userDto);
        return true;
    } catch (error) {
        console.log(error);
        return false;
    }
}