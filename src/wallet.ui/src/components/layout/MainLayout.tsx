import React from 'react';
import { Layout, Menu } from 'antd';
import { Outlet, Link, useLocation } from 'react-router-dom';
import { DashboardOutlined, TransactionOutlined, UserOutlined } from '@ant-design/icons';

const { Header, Content, Footer, Sider } = Layout;

const MainLayout = () => {
  const location = useLocation();

  const menuItems = [
    {
      key: '/',
      icon: <DashboardOutlined />,
      label: <Link to="/">Dashboard</Link>,
    },
    {
      key: '/transactions',
      icon: <TransactionOutlined />,
      label: <Link to="/transactions">Transactions</Link>,
    },
    {
      key: '/profile',
      icon: <UserOutlined />,
      label: <Link to="/profile">Profile</Link>,
    },
  ];

  return (
    <Layout className="min-h-screen">
      <Header className="bg-white border-b">
        <div className="container mx-auto">
          <h1 className="text-2xl">Wallet App</h1>
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