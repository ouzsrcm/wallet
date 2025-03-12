import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:5001/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

// Request interceptor - token ekleme
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Response interceptor - hata yönetimi
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Token geçersiz, kullanıcıyı logout yap
            localStorage.removeItem('token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default api; 