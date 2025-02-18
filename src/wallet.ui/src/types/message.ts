export interface IMessage {
  id: string;
  senderId: string;
  senderUsername: string;
  senderFullName: string;
  receiverId: string;
  receiverFullName: string;
  subject: string;
  content: string;
  isRead: boolean;
  readAt: string | null;
  createdDate: string;
  parentMessageId: string | null;
  replies?: IMessage[];
}

export interface IMessageCreate {
  receiverUsername: string;
  subject: string;
  content: string;
  parentMessageId?: string;
} 