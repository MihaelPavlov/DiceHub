import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantContextService {
  private readonly STORAGE_KEY = 'dicehub:lastTenantId';
  private readonly NAME_STORAGE_KEY = 'dicehub:lastTenantName';

  private _tenantId: string | null = null;
  private _tenantName: string | null = null;
  constructor() {
    this._tenantId = localStorage.getItem(this.STORAGE_KEY);
    this._tenantName = localStorage.getItem(this.NAME_STORAGE_KEY);

    if (this._tenantId === 'system') {
      this.clearTenant();
    }
  }

  get tenantId(): string | null {
    return this._tenantId;
  }

  get tenantName(): string | null {
    return this._tenantName;
  }

  set tenantId(id: string | null) {
    if (id === 'system') {
      this.clearTenant();
      return;
    }

    if (id !== this._tenantId) {
      this._tenantName = null;
      localStorage.removeItem(this.NAME_STORAGE_KEY);
    }

    this._tenantId = id;

    if (id) {
      localStorage.setItem(this.STORAGE_KEY, id);
    } else {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  public setTenant(id: string, name: string): void {
    this.tenantId = id;
    this._tenantName = name;
    localStorage.setItem(this.NAME_STORAGE_KEY, name);
  }

  public clearTenant(): void {
    this._tenantId = null;
    this._tenantName = null;
    localStorage.removeItem(this.STORAGE_KEY);
    localStorage.removeItem(this.NAME_STORAGE_KEY);
  }

  public hasTenant(): boolean {
    return !!this._tenantId && this._tenantId !== 'system';
  }
}
