import React, { useState } from 'react';
import { List, Typography, Space, Button, Modal, Form, Tooltip } from 'antd';
import { 
  MessageOutlined, 
  CheckCircleOutlined, 
  EyeOutlined,
  EyeInvisibleOutlined,
  DeleteOutlined 
} from '@ant-design/icons';
import { IMessage } from '../../types/message';
import NewMessageForm from './NewMessageForm';
import { messageService } from '../../services/messageService';

const { Text } = Typography;

interface MessageItemProps {
  message: IMessage;
  onReply: () => void;
  onStatusChange: () => void;
  onDelete?: (messageId: string) => void;
  currentUserId?: string;
}

const MessageItem: React.FC<MessageItemProps> = ({ 
  message, 
  onReply, 
  onStatusChange,
  onDelete,
  currentUserId 
}) => {
  const [replyModalVisible, setReplyModalVisible] = useState(false);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const handleReply = async (values: any) => {
    try {
      await messageService.sendMessage({
        ...values,
        parentMessageId: message.id
      });
      setReplyModalVisible(false);
      form.resetFields();
      onReply();
    } catch (error) {
      console.error('Reply failed:', error);
    }
  };

  const handleReadStatus = async () => {
    try {
      setLoading(true);
      if (message.isRead) {
        await messageService.markAsUnread(message.id);
      } else {
        await messageService.markAsRead(message.id);
      }
      onStatusChange();
    } catch (error) {
      console.error('Status change failed:', error);
    } finally {
      setLoading(false);
    }
  };

  // Sadece alıcı için okundu/okunmadı butonunu göster
  const showReadButton = currentUserId === message.receiverId;

  return (
    <List.Item
      actions={[
        showReadButton && (
          <Tooltip title={message.isRead ? 'Mark as unread' : 'Mark as read'}>
            <Button 
              key="read" 
              type="text"
              loading={loading}
              icon={message.isRead ? <EyeInvisibleOutlined /> : <EyeOutlined />}
              onClick={handleReadStatus}
              className="hover:text-blue-500 transition-colors"
            />
          </Tooltip>
        ),
        <Tooltip title="Reply">
          <Button 
            key="reply" 
            type="text"
            icon={<MessageOutlined />}
            onClick={() => setReplyModalVisible(true)}
            className="hover:text-blue-500 transition-colors"
          />
        </Tooltip>,
        onDelete && (
          <Tooltip title="Delete">
            <Button 
              key="delete" 
              type="text"
              danger
              icon={<DeleteOutlined />}
              onClick={() => onDelete(message.id)}
              className="hover:text-red-500 transition-colors"
            />
          </Tooltip>
        )
      ].filter(Boolean)}
    >
      <List.Item.Meta
        title={
          <Space>
            <Text strong>{message.subject}</Text>
            <Text type="secondary">
              From: {message.senderFullName}
            </Text>
            {message.isRead && (
              <Tooltip title="Read">
                <CheckCircleOutlined style={{ color: '#52c41a' }} />
              </Tooltip>
            )}
          </Space>
        }
        description={
          <div>
            <Text>{message.content}</Text>
            <div className="mt-2">
              <Text type="secondary">
                {new Date(message.createdDate).toLocaleString()}
              </Text>
            </div>
          </div>
        }
      />

      <Modal
        title="Reply to Message"
        open={replyModalVisible}
        onCancel={() => setReplyModalVisible(false)}
        footer={null}
      >
        <NewMessageForm 
          form={form}
          onFinish={handleReply}
          onCancel={() => setReplyModalVisible(false)}
          initialSubject={`Re: ${message.subject}`}
          initialReceiver={{
            username: message.senderUsername,
            fullName: message.senderFullName
          }}
          isReply={true}
        />
      </Modal>
    </List.Item>
  );
};

export default MessageItem; 