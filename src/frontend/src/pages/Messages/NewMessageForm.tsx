import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Select, Upload, message } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { messageService } from '../../services/messageService';
import { IUser } from '../../types/user';
import { IMessageCreate } from '../../types/message';

const { TextArea } = Input;
const { Option } = Select;

interface NewMessageFormProps {
  form: any;
  onFinish: (values: IMessageCreate) => void;
  onCancel: () => void;
  initialSubject?: string;
  initialReceiver?: {
    username: string;
    fullName: string;
  };
  isReply?: boolean;
  users?: IUser[];
}

const NewMessageForm: React.FC<NewMessageFormProps> = ({
  form,
  onFinish,
  onCancel,
  initialSubject = '',
  initialReceiver,
  isReply = false
}) => {
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState<IUser[]>([]);

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

  const normFile = (e: any) => {
    if (Array.isArray(e)) {
      return e;
    }
    return e?.fileList;
  };

  const beforeUpload = (file: File) => {
    const isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      message.error('Dosya 10MB\'dan küçük olmalıdır!');
    }
    return false; // Otomatik yüklemeyi engelle
  };

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={(values) => {
        const messageData: IMessageCreate = {
          receiverUsername: values.receiverUsername,
          subject: values.subject,
          content: values.content,
          attachment: values.attachment?.[0]?.originFileObj
        };
        onFinish(messageData);
      }}
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
            options={users?.map(user => ({
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

      <Form.Item
        name="attachment"
        label="Dosya Eki"
        valuePropName="fileList"
        getValueFromEvent={normFile}
      >
        <Upload
          beforeUpload={beforeUpload}
          maxCount={1}
          listType="text"
        >
          <Button icon={<UploadOutlined />}>Dosya Seç</Button>
        </Upload>
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