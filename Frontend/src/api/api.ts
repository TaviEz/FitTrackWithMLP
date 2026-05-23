import axios from "axios";

const api = axios.create({
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    withCredentials: true,
    withXSRFToken: true
});

export default api;