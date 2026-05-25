import axios from "axios";

const api = axios.create({
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    withCredentials: true,
    withXSRFToken: true,
    validateStatus: (status) => status < 500
});

export default api;