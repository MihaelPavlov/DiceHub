import { QrCodeType } from "../enums/qr-code-type.enum";

export interface IQrCode {
    Id: number;
    Name: string;
    Type: QrCodeType;
}