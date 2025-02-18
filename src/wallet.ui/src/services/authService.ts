import api from './api';
import { ILoginRequest, ILoginResponse } from '../types/auth';

export const authService = {
    login: async (credentials: ILoginRequest): Promise<ILoginResponse> => {
        const response = await api.post<ILoginResponse>('/Auth/login', credentials);
        return response.data;
    },

    logout: async () => {
        await api.post('/Auth/logout');
        localStorage.removeItem('token');
    }
}; 