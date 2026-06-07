import axios from "axios";

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => { accessToken = token; };
export const getAccessToken = () => accessToken;

const API_BASE_URL = import.meta.env.VITE_USER_API_URL
    ? `${import.meta.env.VITE_USER_API_URL}/user`
    : 'http://localhost:8081/api/user';

const api = axios.create({
    withCredentials: true
});

api.interceptors.request.use(config => {
    if (accessToken) {
        config.headers['Authorization'] = `Bearer ${accessToken}`;
    }
    return config;
});

api.interceptors.response.use(null, async error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 &&
        !originalRequest._retry &&
        !originalRequest.url?.includes('/refresh')) {

        originalRequest._retry = true;
        try {
            const result = await api.post(`${API_BASE_URL}/refresh`);
            setAccessToken(result.data.accessToken);
            originalRequest.headers['Authorization'] = `Bearer ${result.data.accessToken}`;
            return api(originalRequest);
        } catch {
            setAccessToken(null);
            return Promise.reject(error);
        }
    }

    return Promise.reject(error);
});

export default api;