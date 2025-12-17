import { Observable } from 'rxjs';
import { ITenantListResult } from '../../entities/common/models/tenant-list.model';
import { PATH } from '../configs/path.config';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantContextService {
  private _tenantId: string | null = null;

  get tenantId(): string | null {
    return this._tenantId;
  }

  set tenantId(id: string | null) {
    this._tenantId = id;
  }

  public clearTenant(): void {
    this._tenantId = null;
  }

  public hasTenant(): boolean {
    return !!this._tenantId;
  }
}
