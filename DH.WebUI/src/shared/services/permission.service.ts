import { Injectable } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { UserAction } from '../constants/user-action';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private authService: AuthService) {}

  public hasUserAction(
    permissions: UserAction | UserAction[]
  ): Observable<boolean> {
    const permissionsArray = Array.isArray(permissions)
      ? permissions
      : [permissions];

    return this.authService.userInfo$.pipe(
      map((userInfo) => {
        const permissionString = userInfo?.permissionString || '';
        return permissionsArray.some(
          (permission) => permissionString.charAt(Number(permission)) === '1'
        );
      })
    );
  }
}
