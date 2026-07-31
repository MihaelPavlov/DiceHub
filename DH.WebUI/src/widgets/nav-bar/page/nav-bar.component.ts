import { Component, Input } from '@angular/core';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

export interface Column {
  name: string;
  link: string;
  isActive: boolean;
}

@Component({
  selector: 'app-nav-bar',
  templateUrl: 'nav-bar.component.html',
  styleUrl: 'nav-bar.component.scss',
  standalone: false,
})
export class NavBarComponent {
  @Input() columns: Column[] = [];

  constructor(private readonly tenantRouter: TenantRouter) {}

  public toggleActive(item: Column): void {
    // Don't preemptively mark as active
    this.tenantRouter.navigateTenant(item.link).then((navigated) => {
      if (navigated) {
        // If navigation succeeded, update active states
        this.columns.forEach((column) => (column.isActive = column === item));
      }
    });
  }
}
