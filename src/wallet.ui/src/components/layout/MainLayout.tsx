import { Layout, Menu, Dropdown, Space, Avatar } from 'antd';
import { useLocation } from 'react-router-dom';
import { UserOutlined } from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { menuItems } from '../../config/menuItems';
import { RootState } from '../../store';
import { useUserMenu, UserMenuItem } from './UserMenu';
import { ReactNode } from 'react';


const { Header, Content, Footer, Sider } = Layout;

interface MainLayoutProps {
  children: ReactNode;
}

const MainLayout = ({ children }: MainLayoutProps) => {
  const location = useLocation();
  const { user } = useSelector((state: RootState) => state.auth);
  const userMenuItems = useUserMenu();

  return (
    <Layout className="min-h-screen">
      <Header className="bg-white border-b px-6">
        <div className="flex justify-between items-center h-full">
          <h1 className="text-2xl m-0">Wallet App</h1>
          
          <Dropdown menu={{ items: userMenuItems as UserMenuItem[] }} trigger={['click']}>
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
          {children}
        </Content>
      </Layout>
      <Footer className="text-center">
        Wallet App ©{new Date().getFullYear()}
      </Footer>
    </Layout>
  );
};

export default MainLayout; 