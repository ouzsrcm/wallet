import { UserOutlined, LogoutOutlined, MessageOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { logout } from '../../store/slices/authSlice';
import { authService } from '../../services/authService';

export interface UserMenuItem {
  key?: string;
  icon?: React.ReactNode;
  label?: string | React.ReactNode;
  onClick?: () => void;
  type?: 'divider';
}

export const useUserMenu = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const handleLogout = async () => {
    try {
      await authService.logout();
      dispatch(logout());
      navigate('/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const userMenuItems: UserMenuItem[] = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Profile',
      onClick: () => navigate('/profile'),
    },
    {
      key: 'messages',
      icon: <MessageOutlined />,
      label: 'Messages',
      onClick: () => navigate('/messages'),
    },
    {
      type: 'divider',
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Logout',
      onClick: handleLogout,
    },
  ];

  return userMenuItems;
}; 