import React, { useState, useEffect } from 'react';
import { Card, List, Button, Modal, Form, Input, Select, message, Tabs } from 'antd';
import { MailFilled, InboxOutlined, SendOutlined, DeleteOutlined } from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import { messageService } from '../../services/messageService';
import { IMessage, IMessageCreate } from '../../types/message';
import MessageItem from './MessageItem';
import NewMessageForm from './NewMessageForm';
import MessageThread from './MessageThread';

const { TabPane } = Tabs;

const Messages = () => {
  const [messages, setMessages] = useState<IMessage[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [activeTab, setActiveTab] = useState('inbox');
  const { user } = useSelector((state: RootState) => state.auth);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchMessages();
  }, [activeTab]);

  const fetchMessages = async () => {
    try {
      setLoading(true);
      let response: IMessage[] = [];
      switch (activeTab) {
        case 'sent':
          response = await messageService.getSentMessages();
          break;
        case 'trash':
          // TODO: Implement trash messages endpoint
          response = [];
          break;
        default: // inbox
          response = await messageService.getInboxMessages();
      }
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

  const handleStatusChange = () => {
    fetchMessages();
  };

  const organizeMessages = (messages: IMessage[]): IMessage[] => {
    const messageMap = new Map<string, IMessage>();
    
    // Tüm mesajları map'e ekle
    messages.forEach(msg => {
      messageMap.set(msg.id, { ...msg, replies: [] });
    });

    // Yanıtları parent mesajlara ekle
    messages.forEach(msg => {
      if (msg.parentMessageId) {
        const parent = messageMap.get(msg.parentMessageId);
        if (parent && parent.replies) {
          parent.replies.push(messageMap.get(msg.id)!);
        }
      }
    });

    // Sadece parent mesajları döndür
    return Array.from(messageMap.values())
      .filter(msg => !msg.parentMessageId)
      .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-bold">Messages</h2>
        <Button 
          type="primary"
          onClick={() => setIsModalVisible(true)}
          className="
            h-10
            bg-purple-600 
            hover:bg-purple-700 
            transition-all 
            duration-300 
            transform 
            hover:scale-105 
            shadow-md 
            hover:shadow-xl
            border-none
          "
          style={{ 
            background: 'linear-gradient(145deg, #9333EA, #7C3AED)',
            borderColor: 'transparent'
          }}
        >
          <span className="flex items-center gap-2">
            <MailFilled className="text-white" />
            New Message
          </span>
        </Button>
      </div>

      <Tabs 
        activeKey={activeTab} 
        onChange={setActiveTab}
        className="message-tabs bg-sky-50 p-3 rounded-lg shadow-sm"
        tabBarStyle={{
          marginBottom: 16,
          padding: '8px 16px',
          borderRadius: '8px',
          backgroundColor: '#f0f9ff',
        }}
        items={[
          {
            key: 'inbox',
            label: (
              <span className="flex items-center gap-2 px-3 py-1">
                <InboxOutlined />
                Inbox
              </span>
            ),
            children: (
              <Card>
                <List
                  loading={loading}
                  itemLayout="horizontal"
                  dataSource={organizeMessages(messages)}
                  renderItem={(item) => (
                    <MessageThread
                      message={item}
                      onReply={fetchMessages}
                      onStatusChange={handleStatusChange}
                      currentUserId={user?.id}
                    />
                  )}
                />
              </Card>
            )
          },
          {
            key: 'sent',
            label: (
              <span className="flex items-center gap-2 px-3 py-1">
                <SendOutlined />
                Sent
              </span>
            ),
            children: (
              <Card>
                <List
                  loading={loading}
                  itemLayout="horizontal"
                  dataSource={organizeMessages(messages)}
                  renderItem={(item) => (
                    <MessageThread
                      message={item}
                      onReply={fetchMessages}
                      onStatusChange={handleStatusChange}
                      currentUserId={user?.id}
                    />
                  )}
                />
              </Card>
            )
          },
          {
            key: 'trash',
            label: (
              <span className="flex items-center gap-2 px-3 py-1">
                <DeleteOutlined />
                Recycle Bin
              </span>
            ),
            children: (
              <Card>
                <List
                  loading={loading}
                  itemLayout="horizontal"
                  dataSource={organizeMessages(messages)}
                  renderItem={(item) => (
                    <MessageThread
                      message={item}
                      onReply={fetchMessages}
                      onStatusChange={handleStatusChange}
                      currentUserId={user?.id}
                    />
                  )}
                />
              </Card>
            )
          }
        ]}
      />

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