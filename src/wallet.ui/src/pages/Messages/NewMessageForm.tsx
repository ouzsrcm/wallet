import React from 'react';
import { Form, Input, Button, Space } from 'antd';
import { FormInstance } from 'antd/lib/form';

const { TextArea } = Input;

interface NewMessageFormProps {
  form: FormInstance;
  onFinish: (values: any) => void;
  onCancel: () => void;
  initialSubject?: string;
}

const NewMessageForm: React.FC<NewMessageFormProps> = ({
  form,
  onFinish,
  onCancel,
  initialSubject = ''
}) => {
  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={onFinish}
      initialValues={{ subject: initialSubject }}
    >
      <Form.Item
        name="receiverUsername"
        label="To (Username)"
        rules={[{ required: true, message: 'Please input receiver username!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="subject"
        label="Subject"
        rules={[{ required: true, message: 'Please input subject!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="content"
        label="Message"
        rules={[{ required: true, message: 'Please input message content!' }]}
      >
        <TextArea rows={4} />
      </Form.Item>

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit">
            Send
          </Button>
          <Button onClick={onCancel}>
            Cancel
          </Button>
        </Space>
      </Form.Item>
    </Form>
  );
};

export default NewMessageForm; 