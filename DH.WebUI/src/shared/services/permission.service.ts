import { Injectable } from "@angular/core";
import { AuthService } from "../../entities/auth/auth.service";
import { UserAction } from "../constants/user-action";

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private authService: AuthService) {}

  public hasUserAction(permissions: UserAction | UserAction[]): boolean {
    const permissionString = this.authService.getUser?.permissionString || '';

    const permissionsArray = Array.isArray(permissions)
      ? permissions
      : [permissions];

    return permissionsArray.some(
      permission => permissionString.charAt(Number(permission)) === '1'
    );
  }
}
