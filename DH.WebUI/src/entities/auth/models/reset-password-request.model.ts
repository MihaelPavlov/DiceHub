export interface IResetPasswordRequest {
  email: string;
  newPassword: string;
  confirmPassword: string;
  token: string;
}
