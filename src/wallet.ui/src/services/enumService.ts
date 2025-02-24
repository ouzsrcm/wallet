import api from './api';
import { TransactionType } from '../types/enums';

export const enumService = {
    getTransactionTypes: async (): Promise<TransactionType[]> => {
        const response = await api.get('/Enums/transaction-types');
        return response.data;
    }
};

