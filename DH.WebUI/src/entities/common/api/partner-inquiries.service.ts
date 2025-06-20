import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IPartnerInquiryRequest } from '../models/partner-inquiry.model';

@Injectable({
  providedIn: 'root',
})
export class PartnerInquiriesService {
  constructor(private readonly api: RestApiService) {}

  public create(request: IPartnerInquiryRequest): Observable<any> {
    return this.api.post<any>(`/${PATH.PARTNER_INQUIRIES.CORE}`, request);
  }
}
