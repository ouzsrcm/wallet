export interface CategoryDto {
    id: string;
    name: string;
    type: string;
    icon: string;
    color: string;
    description?: string;
    parentCategoryId?: string;
    isSystem?: boolean;
    // Add other fields as necessary
} 