import { QrCodeType } from '../enums/qr-code-type.enum';

export interface IQrCodeValidationResult {
  objectId: number;
  type: QrCodeType;
  isValid: boolean;
  ErrorMessage: string;
}
