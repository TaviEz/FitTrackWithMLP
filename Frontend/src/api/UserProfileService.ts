import axios from "axios";
import UserDetails from "../models/UserDetails";
import UserDetailsDto from "../dtos/UserDetailsDto";
import { getActivityLevelEnum } from "../utils/types";

const API_BASE_URL = 'http://localhost:8081';

export const loginUser = async (email: string, password: string): Promise<any> => {
    try {
        const loginDto = {email: email, password: password, twoFactorCode: '', twoFactorRecoveryCode: ''};
        const result = await axios.post(`${API_BASE_URL}/login`, loginDto);
        return { success: true, token: result.data.accessToken };
    } catch (error) {
        console.log(error);
        return { success: false, error};
    }
}

export const registerUser = async (email: string, password: string): Promise<any> => {
    try {
        const registerDto = {email: email, password: password};
        await axios.post(`${API_BASE_URL}/register`, registerDto);
        return { success: true };
    } catch (error: any) {
        console.log(error);
        const errors = error.response?.data?.errors || {};
        return { success: false, errors };
    }
}

export const getLoggedUserId = async (token: string): Promise<any> => {
    try {
        const result = await axios.get(`${API_BASE_URL}/api/user/me`, {
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
        const userDto = new UserDetailsDto(userId, 
            userDetails.gender, 
            userDetails.age, 
            userDetails.weight, 
            userDetails.height, 
            userActivityLevel,
            userDetails.bmr,
            userDetails.tdee
        )

        await axios.post(`${API_BASE_URL}/api/user/details`, userDto);
        
    } catch (error: any) {
        
    }
}