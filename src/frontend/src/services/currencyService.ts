import api from './api';
import { CurrencyDto } from '../types/Currency';

const currencyService = {
    getAll: async (): Promise<CurrencyDto[]> => {
        const response = await api.get('/v1/Currencies');
        return response.data;
    },
    getById: async (id: string): Promise<CurrencyDto> => {
        const response = await api.get(`/v1/Currencies/${id}`);
        return response.data;
    },
    create: async (currency: CurrencyDto): Promise<CurrencyDto> => {
        const response = await api.post('/v1/Currencies', currency);
        return response.data;
    },
    update: async (id: string, currency: CurrencyDto): Promise<CurrencyDto> => {
        const response = await api.put(`/v1/Currencies/${id}`, currency);
        return response.data;
    },
    delete: async (id: string): Promise<void> => {
        await api.delete(`/v1/Currencies/${id}`);
    }
};

export default currencyService;
