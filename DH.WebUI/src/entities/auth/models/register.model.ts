export interface IRegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  deviceToken: string | null;
}
