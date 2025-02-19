import { DashboardOutlined, TransactionOutlined, UserOutlined, MessageOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';

export interface MenuItem {
  key: string;
  icon: React.ReactNode;
  label: React.ReactNode;
}

export const menuItems: MenuItem[] = [
  {
    key: '/',
    icon: <DashboardOutlined />,
    label: <Link to="/">Dashboard</Link>,
  },
  {
    key: '/profile',
    icon: <UserOutlined />,
    label: <Link to="/profile">Profile</Link>,
  },
  {
    key: '/messages',
    icon: <MessageOutlined />,
    label: <Link to="/messages">Messages</Link>,
  },
]; 