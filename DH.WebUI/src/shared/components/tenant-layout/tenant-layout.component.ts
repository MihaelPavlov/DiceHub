import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { TenantService } from '../../services/tenant.service';

@Component({
  selector: 'app-tenant-layout',
  template: `<router-outlet></router-outlet>`,
  imports: [RouterOutlet],
})
export class TenantLayoutComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private tenantService: TenantService
  ) {}

  ngOnInit(): void {
    console.log('TenantLayout - ngOnInit');
    const tenantId = this.route.snapshot.paramMap.get('tenant');
    console.log('TenantLayout - ', tenantId);

    this.tenantService.tenantId = tenantId;
  }
}
