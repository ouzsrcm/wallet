import api from './api';
import { ILoginRequest, ILoginResponse, IRegisterRequest, IRegisterResponse } from '../types/auth';

const authService = {
    login: async (credentials: ILoginRequest): Promise<ILoginResponse> => {
        const response = await api.post('/auth/login', credentials);
        return response.data;
    },

    register: async (data: IRegisterRequest): Promise<IRegisterResponse> => {
        const response = await api.post('/auth/register', data);
        return response.data;
    },

    logout: async (): Promise<void> => {
        await api.post('/auth/logout');
    }
};

export { authService }; 