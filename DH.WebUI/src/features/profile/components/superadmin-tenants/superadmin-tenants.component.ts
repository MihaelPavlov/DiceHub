import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TenantService } from '../../../../shared/services/tenant.service';
import { ITenantListResult } from '../../../../entities/common/models/tenant-list.model';

@Component({
  selector: 'app-superadmin-tenants',
  templateUrl: './superadmin-tenants.component.html',
  styleUrl: './superadmin-tenants.component.scss',
  standalone: false,
})
export class SuperadminTenantsComponent implements OnInit {
  public tenants: ITenantListResult[] = [];
  public isLoading = true;

  constructor(private readonly tenantService: TenantService, private readonly router: Router) {}

  public ngOnInit(): void {
    this.tenantService.getList().subscribe({
      next: (tenants) => {
        this.tenants = tenants ?? [];
        this.isLoading = false;
      },
      error: () => (this.isLoading = false),
    });
  }

  public open(tenant: ITenantListResult): void {
    this.router.navigate(['/admin/tenants', tenant.id]);
  }
}
