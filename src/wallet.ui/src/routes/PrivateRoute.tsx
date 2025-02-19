import { Navigate, Outlet } from 'react-router-dom';
import { useSelector } from 'react-redux';
import MainLayout from '../components/layout/MainLayout';
import { RootState } from '../store';

const PrivateRoute = () => {
    const { isAuthenticated } = useSelector((state: RootState) => state.auth);

    if (!isAuthenticated) {
        return <Navigate to="/auth/login" replace />;
    }

    return (
        <MainLayout>
            <Outlet />
        </MainLayout>
    );
};

export default PrivateRoute; 