export interface PersonData {
    id: string;
    firstName: string;
    lastName: string;
    middleName?: string;
    dateOfBirth: string;
    gender: string;
    language?: string;
    timeZone?: string;
    currency?: string;
    addresses: PersonAddress[];
    contacts: PersonContact[];
}

export interface PersonAddress {
    id: string;
    addressType?: string;
    addressName?: string;
    addressLine1?: string;
    addressLine2?: string;
    district?: string;
    city?: string;
    state?: string;
    country: string;
    postalCode: string;
    isDefault: boolean;
}

export interface PersonContact {
    id: string;
    contactType?: string;
    contactName?: string;
    contactValue?: string;
    countryCode?: string;
    areaCode?: string;
    isDefault: boolean;
    isPrimary: boolean;
} 