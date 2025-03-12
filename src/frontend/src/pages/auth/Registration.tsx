import React from 'react';
import { Button, Form, Input, Card, message } from 'antd';
import { Link, useNavigate } from 'react-router-dom';
import { UserOutlined, MailOutlined, LockOutlined } from '@ant-design/icons';
import { authService } from '../../services/authService';

interface RegistrationForm {
  email: string;
  username: string;
  password: string;
  confirmPassword: string;
}

const Registration: React.FC = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();

  const onFinish = async (values: RegistrationForm) => {
    try {
      if (values.password !== values.confirmPassword) {
        message.error('Şifreler eşleşmiyor');
        return;
      }

      await authService.register({
        email: values.email,
        username: values.username,
        password: values.password
      });

      message.success('Kayıt başarılı! Giriş yapabilirsiniz.');
      navigate('/auth/login');
    } catch (error: any) {
      message.error(error.message || 'Kayıt işlemi başarısız');
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
        <h2 style={{ textAlign: 'center', marginBottom: 24 }}>Kayıt Ol</h2>
        <Form
          form={form}
          name="registration"
          onFinish={onFinish}
          layout="vertical"
        >
          <Form.Item
            name="email"
            rules={[
              { required: true, message: 'Lütfen email adresinizi girin' },
              { type: 'email', message: 'Geçerli bir email adresi girin' }
            ]}
          >
            <Input 
              prefix={<MailOutlined />} 
              placeholder="Email" 
            />
          </Form.Item>

          <Form.Item
            name="username"
            rules={[
              { required: true, message: 'Lütfen kullanıcı adınızı girin' },
              { min: 3, message: 'Kullanıcı adı en az 3 karakter olmalıdır' }
            ]}
          >
            <Input 
              prefix={<UserOutlined />} 
              placeholder="Kullanıcı Adı" 
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              { required: true, message: 'Lütfen şifrenizi girin' },
              { min: 6, message: 'Şifre en az 6 karakter olmalıdır' }
            ]}
          >
            <Input.Password 
              prefix={<LockOutlined />} 
              placeholder="Şifre" 
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            rules={[
              { required: true, message: 'Lütfen şifrenizi tekrar girin' },
              { min: 6, message: 'Şifre en az 6 karakter olmalıdır' }
            ]}
          >
            <Input.Password 
              prefix={<LockOutlined />} 
              placeholder="Şifre Tekrar" 
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" block>
              Kayıt Ol
            </Button>
          </Form.Item>

          <div style={{ textAlign: 'center' }}>
            Zaten hesabınız var mı? <Link to="/auth/login">Giriş Yap</Link>
          </div>
        </Form>
      </Card>
    </div>
  );
};

export default Registration; 