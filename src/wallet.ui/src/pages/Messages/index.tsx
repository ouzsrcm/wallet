import React, { useState, useEffect } from 'react';
import { Card, List, Button, Modal, Form, Input, Select, message } from 'antd';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import { messageService } from '../../services/messageService';
import { IMessage, IMessageCreate } from '../../types/message';
import MessageItem from './MessageItem';
import NewMessageForm from './NewMessageForm';

const { TextArea } = Input;

const Messages = () => {
  const [messages, setMessages] = useState<IMessage[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const { user } = useSelector((state: RootState) => state.auth);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchMessages();
  }, []);

  const fetchMessages = async () => {
    try {
      setLoading(true);
      const response = await messageService.getMessages();
      setMessages(response);
    } catch (error) {
      message.error('Failed to fetch messages');
    } finally {
      setLoading(false);
    }
  };

  const handleNewMessage = async (values: IMessageCreate) => {
    try {
      await messageService.sendMessage(values);
      message.success('Message sent successfully');
      setIsModalVisible(false);
      form.resetFields();
      fetchMessages();
    } catch (error) {
      message.error('Failed to send message');
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold">Messages</h2>
        <Button type="primary" onClick={() => setIsModalVisible(true)}>
          New Message
        </Button>
      </div>

      <Card>
        <List
          loading={loading}
          itemLayout="horizontal"
          dataSource={messages}
          renderItem={(item) => (
            <MessageItem 
              message={item} 
              onReply={fetchMessages}
              currentUserId={user?.id}
            />
          )}
        />
      </Card>

      <Modal
        title="New Message"
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        footer={null}
      >
        <NewMessageForm 
          form={form}
          onFinish={handleNewMessage}
          onCancel={() => setIsModalVisible(false)}
        />
      </Modal>
    </div>
  );
};

export default Messages; 