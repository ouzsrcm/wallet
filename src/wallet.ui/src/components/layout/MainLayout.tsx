import { Layout, Menu, Dropdown, Space, Avatar } from 'antd';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { UserOutlined, LogoutOutlined } from '@ant-design/icons';
import { useDispatch, useSelector } from 'react-redux';
import { menuItems, MenuItem } from '../../config/menuItems';
import { RootState } from '../../store';
import { logout } from '../../store/slices/authSlice';
import { authService } from '../../services/authService';

const { Header, Content, Footer, Sider } = Layout;

const MainLayout = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { user } = useSelector((state: RootState) => state.auth);

  const handleLogout = async () => {
    try {
      await authService.logout();
      dispatch(logout());
      navigate('/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Profile',
      onClick: () => navigate('/profile'),
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

  return (
    <Layout className="min-h-screen">
      <Header className="bg-white border-b px-6">
        <div className="flex justify-between items-center h-full">
          <h1 className="text-2xl m-0">Wallet App</h1>
          
          <Dropdown menu={{ items: userMenuItems as MenuItem[] }} trigger={['click']}>
            <Space className="cursor-pointer">
              <Avatar 
                icon={<UserOutlined />} 
                className="bg-primary"
              />
              <span className="text-base">
                {user?.firstName} {user?.lastName}
              </span>
            </Space>
          </Dropdown>
        </div>
      </Header>
      <Layout>
        <Sider width={200} className="bg-white">
          <Menu
            mode="inline"
            selectedKeys={[location.pathname]}
            className="h-full border-r"
            items={menuItems}
          />
        </Sider>
        <Content className="p-6">
          <Outlet />
        </Content>
      </Layout>
      <Footer className="text-center">
        Wallet App ©{new Date().getFullYear()}
      </Footer>
    </Layout>
  );
};

export default MainLayout; 