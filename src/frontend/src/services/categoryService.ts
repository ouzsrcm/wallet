import api from './api';
import { CategoryDto } from '../types/category';

const categoryService = {
    getAll: async (): Promise<CategoryDto[]> => {
        const response = await api.get('/v1/Categories');
        return response.data;
    },
    create: async (category: CategoryDto): Promise<CategoryDto> => {
        const response = await api.post('/v1/Categories', category);
        return response.data;
    },
    update: async (id: string, category: CategoryDto): Promise<CategoryDto> => {
        const response = await api.put(`/v1/Categories/${id}`, category);
        return response.data;
    },
    delete: async (id: string): Promise<void> => {
        await api.delete(`/v1/Categories/${id}`);
    }
};

export default categoryService; 