import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { SpaceManagementService } from '../../../entities/space-management/api/space-management.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Injectable({
  providedIn: 'root',
})
export class UserHasActiveTableGuard implements CanActivate {
  constructor(
    private readonly tableService: SpaceManagementService,
    private readonly tenantRouter: TenantRouter
  ) {}

  canActivate(): Observable<boolean> {
    return this.tableService.getUserActiveTable().pipe(
      map((userActiveSpaceTable) => {
        if (userActiveSpaceTable.activeTableId) {
         this.tenantRouter.navigateTenant(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
          return false;
        }
        return true;
      })
    );
  }
}
