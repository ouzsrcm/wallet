import { MenuProps } from 'antd';

export type UserMenuItem = Required<MenuProps>['items'][number];

export const useUserMenu = () => {
    const userMenuItems: UserMenuItem[] = [
        {
            key: 'profile',
            label: 'Profile',
        },
        {
            key: 'logout',
            label: 'Logout',
            danger: true,
        }
    ];

    return userMenuItems;
}; 