
export interface CurrencyDto {
    id: string;
    code: string;
    name: string;
    symbol: string;
    flag: string;
    isActive: boolean;
    isDefault: boolean;
    decimalPlaces: number;
    format: string;
    exchangeRate: number;
    lastUpdated: Date;
}

