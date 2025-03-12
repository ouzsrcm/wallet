import { useState } from 'react';
import { Card, Form, Input, Button, message } from 'antd';
import { MailOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';
import { authService } from '../../services/authService';

const ForgotPassword = () => {
    const [loading, setLoading] = useState(false);

    const onFinish = async (values: { email: string }) => {
        try {
            setLoading(true);
            await authService.forgotPassword(values.email);
            message.success('Password reset link sent to your email.');
        } catch (error: any) {
            message.error(error.response?.data?.message || 'An error occurred.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            minHeight: '100vh',
            background: '#f0f2f5' 
        }}>
            <Card style={{ width: 400 }}>
                <h2 style={{ textAlign: 'center', marginBottom: 24 }}>Remember password</h2>
                <Form
                    name="forgotPassword"
                    onFinish={onFinish}
                    layout="vertical"
                >
                    <Form.Item
                        name="email"
                        rules={[
                            { required: true, message: 'Please enter your email!' },
                            { type: 'email', message: 'Please enter a valid email!' }
                        ]}
                    >
                        <Input
                            prefix={<MailOutlined />}
                            placeholder="Email"
                            size="large"
                        />
                    </Form.Item>

                    <Form.Item>
                        <Button
                            type="primary"
                            htmlType="submit"
                            className="w-full"
                            size="large"
                            loading={loading}
                        >
                            Reset password
                        </Button>
                    </Form.Item>

                    <div style={{ textAlign: 'center' }}>
                        <Link to="/auth/login">Go to login page</Link>
                    </div>
                </Form>
            </Card>
        </div>
    );
};

export default ForgotPassword; 