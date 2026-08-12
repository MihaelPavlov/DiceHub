export interface IRegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  tenantId: string | null;
  deviceToken: string | null;
  language: string;
}
