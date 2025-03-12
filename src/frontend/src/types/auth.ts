export interface IUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  token: string;
}

export interface ILoginRequest {
  username: string;
  password: string;
  rememberMe?: boolean;
}

export interface ILoginResponse {
  user: IUser;
  token: string;
  refreshToken: string;
}

export interface IRegisterRequest {
  email: string;
  username: string;
  password: string;
}

export interface IRegisterResponse {
  success: boolean;
  message: string;
} 