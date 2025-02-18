import api from './api';
import { IMessage, IMessageCreate } from '../types/message';

export const messageService = {
  getMessages: async (): Promise<IMessage[]> => {
    const response = await api.get<IMessage[]>('/messages/inbox');
    return response.data;
  },

  getSentMessages: async (): Promise<IMessage[]> => {
    const response = await api.get<IMessage[]>('/messages/sent');
    return response.data;
  },

  sendMessage: async (message: IMessageCreate): Promise<IMessage> => {
    const response = await api.post<IMessage>('/messages', message);
    return response.data;
  },

  markAsRead: async (messageId: string): Promise<void> => {
    await api.put(`/messages/${messageId}/read`);
  },

  markAsUnread: async (messageId: string): Promise<void> => {
    await api.put(`/messages/${messageId}/unread`);
  },

  deleteMessage: async (messageId: string): Promise<void> => {
    await api.delete(`/messages/${messageId}`);
  },

  deleteThread: async (messageId: string): Promise<void> => {
    await api.delete(`/messages/${messageId}/thread`);
  }
}; 