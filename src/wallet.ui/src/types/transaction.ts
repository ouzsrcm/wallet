export interface TransactionDto {
    id: string;
    description: string;
    categoryId: string;
    amount: number;
    currency: string;
    transactionDate: Date;
    type: string;
    paymentMethod: string;
    reference: string;
    isRecurring: boolean;
    recurringPeriod: string;
} 