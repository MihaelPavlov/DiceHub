import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { SpaceManagementService } from '../../../entities/space-management/api/space-management.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';

@Injectable({
  providedIn: 'root',
})
export class UserHasActiveTableGuard implements CanActivate {
  constructor(
    private readonly tableService: SpaceManagementService,
    private readonly router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.tableService.getUserActiveTable().pipe(
      map((userActiveSpaceTable) => {
        if (userActiveSpaceTable.activeTableId) {
          this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
          return false;
        }
        return true;
      })
    );
  }
}
