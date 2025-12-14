import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantService {
  private _tenantId: string | null = null;

  get tenantId(): string | null {
    return this._tenantId;
  }

  set tenantId(id: string | null) {
    this._tenantId = id;
  }
}
