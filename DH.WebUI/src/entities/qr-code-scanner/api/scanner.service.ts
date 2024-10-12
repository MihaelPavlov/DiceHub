import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Observable } from 'rxjs';
import { IQrCodeRequest } from '../models/qr-code-request.model';
import { IQrCodeValidationResult } from '../models/qr-code-validation-result.model';

@Injectable({
  providedIn: 'root',
})
export class ScannerService {
  constructor(private readonly api: RestApiService) {}

  public upload(
    request: IQrCodeRequest
  ): Observable<IQrCodeValidationResult | null> {
    return this.api.post<IQrCodeValidationResult>(
      `/${PATH.SCANNER.CORE}/${PATH.SCANNER.UPLOAD}`,
      request
    );
  }
}
