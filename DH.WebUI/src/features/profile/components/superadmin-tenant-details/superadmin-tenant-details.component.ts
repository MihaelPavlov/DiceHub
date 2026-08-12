import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ITenantListResult } from '../../../../entities/common/models/tenant-list.model';
import { TenantService } from '../../../../shared/services/tenant.service';
import { TenantContextService } from '../../../../shared/services/tenant-context.service';

@Component({ selector: 'app-superadmin-tenant-details', templateUrl: './superadmin-tenant-details.component.html', styleUrl: './superadmin-tenant-details.component.scss', standalone: false })
export class SuperadminTenantDetailsComponent implements OnInit {
  public tenant?: ITenantListResult;
  public isLoading = true;

  constructor(private readonly route: ActivatedRoute, private readonly router: Router, private readonly tenantService: TenantService, private readonly tenantContext: TenantContextService) {}

  public ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.router.navigate(['/admin/tenants']); return; }
    this.tenantService.getById(id).subscribe({ next: (tenant) => { this.tenant = tenant; this.isLoading = false; }, error: () => { this.isLoading = false; } });
  }

  public visit(): void {
    if (!this.tenant) return;
    this.tenantContext.tenantId = this.tenant.id;
    this.router.navigateByUrl(`/${this.tenant.id}/games/library`);
  }

  public back(): void { this.router.navigate(['/admin/tenants']); }
}
