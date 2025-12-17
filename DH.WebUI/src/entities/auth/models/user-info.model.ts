import { UserRole } from '../enums/roles.enum';

export interface IUserInfo {
  tenantId: string;
  id: string;
  role: UserRole;
  username: string;
  permissionString: string;
}
