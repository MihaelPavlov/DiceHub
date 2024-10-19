import { Injectable } from '@angular/core';
import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { SpaceManagementService } from '../../entities/space-management/api/space-management.service';

@Injectable({
  providedIn: 'root',
})
export class UserHasActiveTableGuard {
  constructor(
    private readonly spaceManagementService: SpaceManagementService,
    private readonly router: Router
  ) {}

  public async canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    this.spaceManagementService.getUserActiveTable().subscribe({
      next: (x) => {
        if (x.isPlayerHaveActiveTable || x.isPlayerParticipateInTable) {
          this.router.navigateByUrl('space/home');
          return true;
        }

        this.router.navigateByUrl('space/list');
        return false;
      },
    });
  }
}
