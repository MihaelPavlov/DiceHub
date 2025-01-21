import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../entities/auth/auth.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { IUser } from '../../../../entities/profile/models/user.model';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';

@Component({
  selector: 'app-profile',
  templateUrl: 'employee-list.component.html',
  styleUrl: 'employee-list.component.scss',
})
export class EmployeeListComponent implements OnDestroy {
  public employees: IUser[] = [];
    public readonly ImageEntityType = ImageEntityType;
  
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly usersService: UsersService,
    private readonly router: Router
  ) {
    this.usersService.getEmployeeList().subscribe({
      next: (employees) => (this.employees = employees ?? []),
      error: (error) => {
        console.log(error);
      },
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onAdd(): void {
    this.router.navigateByUrl('profile/add-employee');

  }

  public onBack(): void {
    this.router.navigateByUrl('profile');
  }
}
