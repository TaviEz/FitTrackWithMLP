import UserDetails from "../models/UserDetails";
import UserDetailsDto from "../dtos/UserDetailsDto";
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
        console.log(error);
        return { success: false, error};
    }
}

export const registerUser = async (email: string, password: string): Promise<any> => {
    try {
        const registerDto = {email: email, password: password};
        await api.post(`${API_BASE_URL}/register`, registerDto);
        return { success: true };
    } catch (error: any) {
        console.log(error);
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
        console.log(error);
    }
}

export const saveUserDetails = async (userDetails: UserDetails, userId: string): Promise<any> => {
    try {
        const userActivityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
        const userDto = UserDetailsDto.fromDomain(userDetails, userId, userActivityLevel);

        await api.post(`${API_BASE_URL}/api/user/details`, userDto);
        
    } catch (error: any) {
        
    }
}