import api from './api';
import { TransactionType } from '../types/enums';
import { PaymentMethod } from '../types/PaymentMethod';

export const enumService = {
    getTransactionTypes: async (): Promise<TransactionType[]> => {
        const response = await api.get('/Enums/transaction-types');
        return response.data;
    },

    getPaymentMethods: async (): Promise<PaymentMethod[]> => {
        const response = await api.get('/Enums/payment-methods');
        return response.data;
    }
};

