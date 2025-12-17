import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { TenantContextService } from '../../services/tenant-context.service';

@Component({
  selector: 'app-tenant-layout',
  template: `<router-outlet></router-outlet>`,
  imports: [RouterOutlet],
})
export class TenantLayoutComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private tenantContextService: TenantContextService
  ) {}

  public ngOnInit(): void {
    const tenantId = this.route.snapshot.paramMap.get('tenant');
    this.tenantContextService.tenantId = tenantId;
  }
}
