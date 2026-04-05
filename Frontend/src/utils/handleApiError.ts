import { AxiosError } from 'axios';

export const handleApiError = (error: unknown): string => {
    const axiosError = error as AxiosError;

    if (axiosError.response) {
        const status = axiosError.response.status;

        switch (status) {
            case 400:
                return "Bad Request";
            case 401:
                return "Unauthorized";
            case 500:
                return "Server error";
            default:
                return `Unexpected error (${status})`;
        }
    } else if (axiosError.request) {
        return "No response from server. Check network or backend status.";
    } else {
        return "Unexpected error";
    }
};
