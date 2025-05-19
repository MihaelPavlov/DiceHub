export interface ICreateEmployeePasswordRequest {
  email: string;
  phoneNumber: string;
  newPassword: string;
  confirmPassword: string;
  token: string;
}
