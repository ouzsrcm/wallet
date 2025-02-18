import React from 'react';
import { IMessage } from '../../types/message';
import MessageItem from './MessageItem';

interface MessageThreadProps {
  message: IMessage;
  onReply: () => void;
  onStatusChange: () => void;
  currentUserId?: string;
  level?: number;
}

const MessageThread: React.FC<MessageThreadProps> = ({
  message,
  onReply,
  onStatusChange,
  currentUserId,
  level = 0
}) => {
  return (
    <div className={`${level > 0 ? 'ml-12 mt-2' : ''}`}>
      <MessageItem 
        message={message}
        onReply={onReply}
        onStatusChange={onStatusChange}
        currentUserId={currentUserId}
      />
      {message.replies?.map(reply => (
        <MessageThread
          key={reply.id}
          message={reply}
          onReply={onReply}
          onStatusChange={onStatusChange}
          currentUserId={currentUserId}
          level={level + 1}
        />
      ))}
    </div>
  );
};

export default MessageThread; 