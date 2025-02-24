import api from './api';
import { TransactionDto } from '../types/transaction';

const transactionService = {
    getAll: async (userId: string): Promise<TransactionDto[]> => {
        const response = await api.get(`/v1/Transactions?userId=${userId}`);
        return response.data;
    },
    create: async (transaction: TransactionDto): Promise<TransactionDto> => {
        const response = await api.post('/v1/Transactions', transaction);
        return response.data;
    },
    update: async (id: string, transaction: TransactionDto): Promise<TransactionDto> => {
        const response = await api.put(`/v1/Transactions/${id}`, transaction);
        return response.data;
    },
    delete: async (id: string): Promise<void> => {
        await api.delete(`/v1/Transactions/${id}`);
    }
};

export default transactionService; 