import { Routes, Route, Navigate } from 'react-router-dom';
import PrivateRoute from './PrivateRoute.tsx';
import MainLayout from '../components/layout/MainLayout';
import Dashboard from '../pages/Dashboard';
import Transactions from '../pages/Transactions';
import Profile from '../pages/Profile';
import Messages from '../pages/Messages';
import Login from '../pages/auth/Login';
import Registration from '../pages/auth/Registration';
import ForgotPassword from '../pages/auth/ForgotPassword';

const AppRoutes = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/auth">
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Registration />} />
        <Route path="forgot-password" element={<ForgotPassword />} />
      </Route>

      {/* Protected Routes */}
      <Route element={<PrivateRoute />}>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/profile/*" element={<Profile />} />
        <Route path="transactions" element={<Transactions />} />
        <Route path="messages" element={<Messages />} />
      </Route>

      {/* Catch all route */}
      <Route path="*" element={<Navigate to="/auth/login" replace />} />
    </Routes>
  );
};

export default AppRoutes; 