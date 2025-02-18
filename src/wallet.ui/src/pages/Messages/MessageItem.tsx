import React, { useState } from 'react';
import { List, Typography, Space, Button, Modal, Form } from 'antd';
import { MessageOutlined } from '@ant-design/icons';
import { IMessage } from '../../types/message';
import NewMessageForm from './NewMessageForm';
import { messageService } from '../../services/messageService';

const { Text } = Typography;

interface MessageItemProps {
  message: IMessage;
  onReply: () => void;
  currentUserId?: string;
}

const MessageItem: React.FC<MessageItemProps> = ({ message, onReply, currentUserId }) => {
  const [replyModalVisible, setReplyModalVisible] = useState(false);
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

  return (
    <List.Item
      actions={[
        <Button 
          key="reply" 
          type="link" 
          icon={<MessageOutlined />}
          onClick={() => setReplyModalVisible(true)}
        >
          Reply
        </Button>
      ]}
    >
      <List.Item.Meta
        title={
          <Space>
            <Text strong>{message.subject}</Text>
            <Text type="secondary">
              From: {message.senderFullName}
            </Text>
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
        />
      </Modal>
    </List.Item>
  );
};

export default MessageItem; 