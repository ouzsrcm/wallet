import { Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from '../components/layout/MainLayout';
import Dashboard from '../pages/Dashboard';
import Transactions from '../pages/Transactions';
import Profile from '../pages/Profile';
import Messages from '../pages/Messages';
import Login from '../pages/auth/Login';
import AuthGuard from '../guards/AuthGuard';
import GuestGuard from '../guards/GuestGuard';

const AppRoutes = () => {
  return (
    <Routes>
      {/* Protected Routes */}
      <Route path="/" element={<AuthGuard><MainLayout /></AuthGuard>}>
        <Route index element={<Dashboard />} />
        <Route path="transactions" element={<Transactions />} />
        <Route path="profile" element={<Profile />} />
        <Route path="messages" element={<Messages />} />
      </Route>

      {/* Public Routes */}
      <Route path="/login" element={<GuestGuard><Login /></GuestGuard>} />
      
      {/* Catch All */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes; 