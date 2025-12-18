import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantContextService {
  private readonly STORAGE_KEY = 'dicehub:lastTenantId';

  private _tenantId: string | null = null;
  constructor() {
    this._tenantId = localStorage.getItem(this.STORAGE_KEY);
  }

  get tenantId(): string | null {
    return this._tenantId;
  }

  set tenantId(id: string | null) {
    this._tenantId = id;

    if (id) {
      localStorage.setItem(this.STORAGE_KEY, id);
    } else {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  public clearTenant(): void {
    this._tenantId = null;
  }

  public hasTenant(): boolean {
    return !!this._tenantId;
  }
}
