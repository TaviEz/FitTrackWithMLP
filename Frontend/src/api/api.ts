import axios from "axios";

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => { accessToken = token; };
export const getAccessToken = () => accessToken;

const api = axios.create({
    withCredentials: true,
    validateStatus: (status) => status < 500
});

api.interceptors.request.use(config => {
    if (accessToken) {
        config.headers['Authorization'] = `Bearer ${accessToken}`;
    }
    return config;
});

export default api;