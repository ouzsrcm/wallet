import api from './api';
import { PersonData, PersonAddress, PersonContact } from '../types/person';

export const personService = {
    // Kişi işlemleri
    getPerson: async (id: string): Promise<PersonData> => {
        const response = await api.get(`/Person/${id}`);
        return response.data;
    },

    updatePerson: async (id: string, data: Partial<PersonData>): Promise<PersonData> => {
        const response = await api.put(`/Person/${id}`, data);
        return response.data;
    },

    // Adres işlemleri
    getAddresses: async (personId: string): Promise<PersonAddress[]> => {
        const response = await api.get(`/Person/${personId}/addresses`);
        return response.data;
    },

    addAddress: async (personId: string, address: Partial<PersonAddress>): Promise<PersonAddress> => {
        const response = await api.post(`/Person/${personId}/address`, address);
        return response.data;
    },

    updateAddress: async (addressId: string, address: Partial<PersonAddress>): Promise<PersonAddress> => {
        const response = await api.put(`/Person/address/${addressId}`, address);
        return response.data;
    },

    deleteAddress: async (addressId: string): Promise<void> => {
        await api.delete(`/Person/address/${addressId}`);
    },

    setDefaultAddress: async (addressId: string): Promise<PersonAddress> => {
        const response = await api.put(`/Person/address/${addressId}/default`);
        return response.data;
    },

    // İletişim işlemleri
    getContacts: async (personId: string): Promise<PersonContact[]> => {
        const response = await api.get(`/Person/${personId}/contacts`);
        return response.data;
    },

    addContact: async (personId: string, contact: Partial<PersonContact>): Promise<PersonContact> => {
        const response = await api.post(`/Person/${personId}/contact`, contact);
        return response.data;
    },

    updateContact: async (contactId: string, contact: Partial<PersonContact>): Promise<PersonContact> => {
        const response = await api.put(`/Person/contact/${contactId}`, contact);
        return response.data;
    },

    deleteContact: async (contactId: string): Promise<void> => {
        await api.delete(`/Person/contact/${contactId}`);
    },

    setDefaultContact: async (contactId: string): Promise<PersonContact> => {
        const response = await api.put(`/Person/contact/${contactId}/default`);
        return response.data;
    }
}; 