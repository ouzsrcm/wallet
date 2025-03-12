import api from './api';
import { IMessage, IMessageCreate } from '../types/Message';
import { IUser } from '../types/User';

export const messageService = {
  getInboxMessages: async (): Promise<IMessage[]> => {
    const response = await api.get<IMessage[]>('/Messages/inbox');
    return response.data;
  },

  getSentMessages: async (): Promise<IMessage[]> => {
    const response = await api.get<IMessage[]>('/Messages/sent');
    return response.data;
  },

  getMessageById: async (messageId: string): Promise<IMessage> => {
    const response = await api.get<IMessage>(`/Messages/${messageId}`);
    return response.data;
  },

  sendMessage: async (message: IMessageCreate): Promise<IMessage> => {
    const formData = new FormData();
    formData.append('receiverUsername', message.receiverUsername);
    formData.append('subject', message.subject);
    formData.append('content', message.content);
    
    if (message.parentMessageId) {
      formData.append('parentMessageId', message.parentMessageId);
    }

    if (message.attachment) {
      formData.append('attachment', message.attachment);
    }

    const response = await api.post<IMessage>('/Messages', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  markAsRead: async (messageId: string): Promise<void> => {
    await api.put(`/Messages/${messageId}/read`);
  },

  markAsUnread: async (messageId: string): Promise<void> => {
    await api.put(`/Messages/${messageId}/unread`);
  },

  deleteMessage: async (messageId: string): Promise<void> => {
    await api.delete(`/Messages/${messageId}`);
  },

  deleteThread: async (messageId: string): Promise<void> => {
    await api.delete(`/Messages/${messageId}/thread`);
  },

  getUsers: async (): Promise<IUser[]> => {
    const response = await api.get<IUser[]>('/Messages/users');
    return response.data;
  },

  downloadAttachment: async (attachmentId: string): Promise<Blob> => {
    const response = await api.get(`/Messages/attachments/${attachmentId}`, {
      responseType: 'blob'
    });
    return response.data;
  },

  getMessageThread: async (messageId: string): Promise<IMessage[]> => {
    const response = await api.get<IMessage[]>(`/Messages/${messageId}/thread`);
    return response.data;
  }
}; 