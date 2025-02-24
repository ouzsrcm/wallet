export enum Gender {
    Unspecified = 0,
    Male = 1,
    Female = 2
}

export enum AddressType {
    Home = "Home",
    Work = "Work",
    Other = "Other"
} 

export enum LanguageProficiency {
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
} 

export interface TransactionType {
    id: number,
    name: string,
    title: string
}
