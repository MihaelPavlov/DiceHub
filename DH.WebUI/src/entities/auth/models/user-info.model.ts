import { UserRole } from '../enums/roles.enum';

export interface IUserInfo {
  id: string;
  role: UserRole;
  username: string;
  permissionString: string;
}
