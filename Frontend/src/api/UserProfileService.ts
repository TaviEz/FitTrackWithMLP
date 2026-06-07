import UserDetails from "../models/UserDetails";
import UserDetailsDto from "../dtos/UserDetails/UserDetailsDto";
import { getActivityLevelEnum } from "../utils/types";
import api, { setAccessToken } from "./api";
import axios from "axios";

const API_BASE_URL = import.meta.env.VITE_USER_API_URL
    ? `${import.meta.env.VITE_USER_API_URL}/user`
    : 'http://localhost:8081/api/user';

export const loginUser = async (email: string, password: string): Promise<any> => {
    try {
        const loginDto = {email: email, password: password};
        const result = await axios.post(`${API_BASE_URL}/login`, loginDto, { withCredentials: true });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, accessToken: result.data.accessToken };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const registerUser = async (email: string, password: string): Promise<any> => {
    try {
        const registerDto = {email: email, password: password};
        const result = await axios.post(`${API_BASE_URL}/register`, registerDto, { withCredentials: true });

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const getLoggedUserId = async (): Promise<any> => {
    try {
        const result = await api.get(`${API_BASE_URL}/me`);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const getUserDetails = async (): Promise<any> => {
    try {
        const result = await api.get<UserDetailsDto>(`${API_BASE_URL}/details`);

        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status, data: result.data };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const logoutUser = async (): Promise<any> => {
    try {
        const result = await api.post(`${API_BASE_URL}/logout`);
        setAccessToken(null);
        return { success: result.status < 400, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const refreshSession = async (): Promise<any> => {
    try {
        // create a separate axios handler for the refresh
        const result = await axios.post(`${API_BASE_URL}/refresh`, {}, { withCredentials: true });
        if (result.status >= 400) {
            return { success: false, status: result.status };
        }
        return { success: true, status: result.status, accessToken: result.data.accessToken };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}

export const saveUserDetails = async (userDetails: UserDetails): Promise<any> => {
    try {
        const userActivityLevel = getActivityLevelEnum(userDetails.activityLevel.label);
        if (!userActivityLevel) {
            return { success: false, status: 400 };
        }

        if (!userDetails.goal) {
            return { success: false, status: 400 };
        }

        const userDto = UserDetailsDto.fromDomain(userDetails, userActivityLevel);
        const result = await api.post(`${API_BASE_URL}/details`, userDto);
        if (result.status >= 400) {
            return { success: false, status: result.status };
        }

        return { success: true, status: result.status };
    } catch (error: any) {
        return { success: false, status: error?.response?.status };
    }
}