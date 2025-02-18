import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Select } from 'antd';
import { messageService } from '../../services/messageService';
import { IUser } from '../../types/user';

const { TextArea } = Input;
const { Option } = Select;

interface NewMessageFormProps {
  form: any;
  onFinish: (values: any) => void;
  onCancel: () => void;
  initialSubject?: string;
  initialReceiver?: {
    username: string;
    fullName: string;
  };
  isReply?: boolean;
}

const NewMessageForm: React.FC<NewMessageFormProps> = ({
  form,
  onFinish,
  onCancel,
  initialSubject = '',
  initialReceiver,
  isReply = false
}) => {
  const [users, setUsers] = useState<IUser[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!isReply) {
      fetchUsers();
    }
  }, [isReply]);

  useEffect(() => {
    if (initialReceiver) {
      form.setFieldsValue({
        receiverUsername: initialReceiver.username
      });
    }
  }, [initialReceiver, form]);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await messageService.getUsers();
      setUsers(response);
    } catch (error) {
      console.error('Failed to fetch users:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={onFinish}
      initialValues={{ 
        subject: initialSubject,
        receiverUsername: initialReceiver?.username
      }}
    >
      <Form.Item
        name="receiverUsername"
        label="To"
        rules={[{ required: true, message: 'Please select a recipient' }]}
      >
        {isReply ? (
          <Input 
            disabled 
            value={`${initialReceiver?.fullName} (${initialReceiver?.username})`}
          />
        ) : (
          <Select
            showSearch
            placeholder="Select a recipient"
            loading={loading}
            optionFilterProp="children"
            filterOption={(input, option) =>
              (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
            }
            options={users.map(user => ({
              value: user.username,
              label: `${user.fullName} (${user.username})`
            }))}
          />
        )}
      </Form.Item>

      <Form.Item
        name="subject"
        label="Subject"
        rules={[{ required: true, message: 'Please input the subject' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="content"
        label="Message"
        rules={[{ required: true, message: 'Please input your message' }]}
      >
        <TextArea rows={4} />
      </Form.Item>

      <Form.Item>
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
          <Button onClick={onCancel}>Cancel</Button>
          <Button type="primary" htmlType="submit">
            Send
          </Button>
        </div>
      </Form.Item>
    </Form>
  );
};

export default NewMessageForm; 