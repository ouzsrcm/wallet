import { DashboardOutlined, TransactionOutlined, UserOutlined, MessageOutlined, AppstoreOutlined } from '@ant-design/icons';
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
    key: '/transactions',
    icon: <TransactionOutlined />,
    label: <Link to="/transactions">Transactions</Link>,
  },
  {
    key: '/messages',
    icon: <MessageOutlined />,
    label: <Link to="/messages">Messages</Link>,
  },
  {
    key: '/categories',
    icon: <AppstoreOutlined />,
    label: <Link to="/categories">Categories</Link>,
  },
]; 