import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Observable } from 'rxjs';
import { IQrCodeRequest } from '../models/qr-code-request.model';
import { IQrCodeValidationResult } from '../models/qr-code-validation-result.model';
import { QrCodeType } from '../enums/qr-code-type.enum';

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

  /**
   * Asks the server for the short opaque token to encode in a QR for this
   * entity. Keeps the QR tiny (~version 1) so a poor camera can read it.
   */
  public issueToken(
    type: QrCodeType,
    entityId: number
  ): Observable<{ token: string } | null> {
    return this.api.post<{ token: string }>(
      `/${PATH.SCANNER.CORE}/${PATH.SCANNER.ISSUE}`,
      { type, entityId }
    );
  }
}
