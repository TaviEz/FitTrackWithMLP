import UserDetails from "../models/UserDetails";
import UserDetailsDto from "../dtos/UserDetails/UserDetailsDto";
import { getActivityLevelEnum } from "../utils/types";
import api from "./api";

const API_BASE_URL = 'http://localhost:8081';

export const loginUser = async (email: string, password: string): Promise<any> => {
    try {
        const loginDto = {email: email, password: password};
        const result = await api.post(`${API_BASE_URL}/api/user/login`, loginDto);

        const token = result.data.accessToken
        localStorage.setItem('token', token);

        return { success: true, token: token };
    } catch (error) {
        return { success: false, error};
    }
}

export const registerUser = async (email: string, password: string): Promise<any> => {
    try {
        const registerDto = {email: email, password: password};
        await api.post(`${API_BASE_URL}/register`, registerDto);
        return { success: true };
    } catch (error: any) {
        const errors = error.response?.data?.errors || {};
        return { success: false, errors };
    }
}

export const getLoggedUserId = async (token: string): Promise<any> => {
    try {
        const result = await api.get(`${API_BASE_URL}/api/user/me`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        return result.data;
    } catch (error: any) {
        return undefined;
    }
}

export const getUserDetails = async (): Promise<UserDetailsDto | null> => {
    try {
        const result = await api.get<UserDetailsDto>(`${API_BASE_URL}/api/user/details`);
        return result.data;
    } catch (error: any) {
        return null;
    }
}

export const saveUserDetails = async (userDetails: UserDetails): Promise<any> => {
    try {
        const userActivityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
        if (!userActivityLevel) {
            return { success: false, message: "Please select a valid activity level." };
        }

        if (!userDetails.goal) {
            return { success: false, message: "Please select a goal before continuing." };
        }

        const userDto = UserDetailsDto.fromDomain(userDetails, userActivityLevel);
        await api.post(`${API_BASE_URL}/api/user/details`, userDto);
        return { success: true };
    } catch (error: any) {
        return { success: false };
    }
}