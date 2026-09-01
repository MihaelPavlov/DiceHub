import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Location } from '@angular/common';
import { Capacitor } from '@capacitor/core';
import { Camera } from '@capacitor/camera';
import jsQR from 'jsqr';
import { ScannerService } from '../../../entities/qr-code-scanner/api/scanner.service';
import { IQrCode } from '../../../entities/qr-code-scanner/models/qr-code.model';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { take } from 'rxjs';
import { Router } from '@angular/router';
import { IQrCodeValidationResult } from '../../../entities/qr-code-scanner/models/qr-code-validation-result.model';
import { MatDialog } from '@angular/material/dialog';
import { ScanResultAdminDialog } from '../../../shared/dialogs/scan-result-admin/scan-result-admin.dialog';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { QrEncryptService } from '../../../shared/services/qr-code-encrypt.service';
import { ScanConfirmDialogComponent } from '../../../features/qr-code-scanner/dialogs/scan-confirm-dialog.component';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-qr-code-scanner',
  templateUrl: 'qr-code-scanner.component.html',
  styleUrl: 'qr-code-scanner.component.scss',
  standalone: false,
})
export class QrCodeScannerComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  @ViewChild('video') videoElement!: ElementRef<HTMLVideoElement>;
  private readonly KEY_AFTER_SCAN_SUCCESS_MESSAGE = 'afterScanSuccessMessage';
  private readonly KEY_AFTER_SCAN_ERROR_MESSAGE = 'afterScanErrorMessage';
  public imageSrc: string | null = null;
  public canvas!: HTMLCanvasElement;
  public context!: CanvasRenderingContext2D | null;
  public invalidQrCode = false;
  public isValidQrScanned = false;
  public currentQrCodeType: QrCodeType | null = null;
  public QrCodeType = QrCodeType;
  public afterScanSuccessfulMessage: string | null = null;
  public afterScanErrorMessage: string | null = null;
  private mediaStream: MediaStream | null = null;

  constructor(
    private readonly scannerService: ScannerService,
    private readonly tenantRouter: TenantRouter,
    private readonly dialog: MatDialog,
    private readonly translateService: TranslateService,
    private readonly qrEncryptService: QrEncryptService,
    private readonly location: Location
  ) {}

  public ngOnInit(): void {
    this.initAfterScanSuccessMessage();
    this.initAfterScanErrorMessage();
  }

  public goBack(): void {
    this.location.back();
  }

  public ngAfterViewInit(): void {
    this.canvas = document.createElement('canvas');
    this.context = this.canvas.getContext('2d');
    this.startCamera();
  }

  public ngOnDestroy(): void {
    this.stopCamera();
  }

  private stopCamera(): void {
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach((track) => track.stop());
      this.mediaStream = null;
    }

    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.pause();
      this.videoElement.nativeElement.srcObject = null;
    }
  }

  private async startCamera(): Promise<void> {
    // On Android the WebView's getUserMedia permission request is auto-denied
    // (no prompt) unless the OS CAMERA permission has been granted first. Ask
    // for it explicitly through the Capacitor plugin so the user sees a dialog.
    if (Capacitor.isNativePlatform()) {
      try {
        let state = (await Camera.checkPermissions()).camera;
        if (state !== 'granted') {
          state = (await Camera.requestPermissions({ permissions: ['camera'] }))
            .camera;
        }
        if (state !== 'granted') {
          this.afterScanErrorMessage = this.translateService.instant(
            'qr_scanner.camera_permission_denied'
          );
          return;
        }
      } catch {
        // fall through - getUserMedia below will surface any remaining error
      }
    }

    navigator.mediaDevices
      .getUserMedia({ video: { facingMode: 'environment' } })
      .then((stream) => {
        try {
          this.mediaStream = stream;

          this.videoElement.nativeElement.srcObject = stream;
          this.videoElement.nativeElement.play();
          requestAnimationFrame(this.tick.bind(this));
        } catch (err) {
          console.log(err);
        }
      })
      .catch((err) => {
        this.afterScanErrorMessage = this.translateService.instant(
          err?.name === 'NotAllowedError'
            ? 'qr_scanner.camera_permission_denied'
            : 'qr_scanner.camera_unavailable'
        );
        console.log(err);
      });
  }

  private tick(): void {
    if (!this.videoElement) {
      return;
    }
    const video = this.videoElement.nativeElement;

    if (video.readyState === video.HAVE_ENOUGH_DATA) {
      this.context?.drawImage(
        video,
        0,
        0,
        this.canvas.width,
        this.canvas.height
      );
      const imageData = this.context?.getImageData(
        0,
        0,
        this.canvas.width,
        this.canvas.height
      );

      if (imageData) {
        const code = jsQR(imageData.data, imageData.width, imageData.height);

        if (code) {
          this.afterScanSuccessfulMessage = null;
          video.pause();
          let decryptQrCode: string | null = null;

          try {
            decryptQrCode = this.qrEncryptService.decryptObjectSync(code.data);
          } catch {
            decryptQrCode = null;
          }

          if (!decryptQrCode || !this.isQrCodeValid(decryptQrCode)) {
            this.invalidQrCode = true;
            video.play();
            setTimeout(() => {
              this.invalidQrCode = false;
            }, 3000);
          } else {
            this.currentQrCodeType = (
              JSON.parse(decryptQrCode) as IQrCode
            ).Type;
            this.invalidQrCode = false;
            const request = { data: code.data };
            this.isValidQrScanned = true;

            const dialogRefConfirmation = this.dialog.open(
              ScanConfirmDialogComponent,
              {
                panelClass: 'confirm-sheet-pane',
                data: {
                  type: this.currentQrCodeType,
                },
              }
            );

            dialogRefConfirmation
              .afterClosed()
              .pipe(take(1))
              .subscribe((confirmed) => {
                if (confirmed) {
                  this.scannerService
                    .upload(request)
                    .pipe(take(1))
                    .subscribe({
                      next: (res: IQrCodeValidationResult | null) => {
                        if (res) {
                          switch (res.type) {
                            case QrCodeType.Game:
                              if (res.isValid) {
                                this.tenantRouter.navigateTenant(
                                  FULL_ROUTE.SPACE_MANAGEMENT.CREATE(
                                    res.objectId
                                  )
                                );
                              } else {
                                this.setLocalStorageErrorMessage(
                                  res.errorMessage
                                );
                                window.location.reload();
                              }
                              break;

                            case QrCodeType.GameReservation:
                              if (res.isValid) {
                                this.setLocalStorageSuccessMessage(
                                  this.translateService.instant(
                                    'qr_scanner.game_reservation_valid',
                                    {
                                      note:
                                        res.internalNote ??
                                        this.translateService.instant(
                                          'qr_scanner.no_staff_note'
                                        ),
                                    }
                                  )
                                );
                                window.location.reload();
                              } else {
                                const dialogRef = this.dialog.open(
                                  ScanResultAdminDialog,
                                  {
                                    panelClass: 'confirm-sheet-pane', data: res,
                                  }
                                );

                                dialogRef.afterClosed().subscribe({
                                  next: () => {
                                    window.location.reload();
                                  },
                                });
                              }
                              break;
                            case QrCodeType.TableReservation:
                              if (res.isValid) {
                                this.setLocalStorageSuccessMessage(
                                  this.translateService.instant(
                                    'qr_scanner.table_reservation_valid',
                                    {
                                      note:
                                        res.internalNote ??
                                        this.translateService.instant(
                                          'qr_scanner.no_staff_note'
                                        ),
                                    }
                                  )
                                );
                                window.location.reload();
                              } else {
                                const dialogRef = this.dialog.open(
                                  ScanResultAdminDialog,
                                  {
                                    panelClass: 'confirm-sheet-pane', data: res,
                                  }
                                );

                                dialogRef.afterClosed().subscribe({
                                  next: () => {
                                    window.location.reload();
                                  },
                                });
                              }
                              break;
                            case QrCodeType.Reward:
                              const dialogRef = this.dialog.open(
                                ScanResultAdminDialog,
                                {
                                  panelClass: 'confirm-sheet-pane', data: res,
                                }
                              );

                              dialogRef.afterClosed().subscribe({
                                next: () => {
                                  window.location.reload();
                                },
                              });
                              break;
                            case QrCodeType.PurchaseChallenge:
                              if (!res.isValid) {
                                const dialogReference = this.dialog.open(
                                  ScanResultAdminDialog,
                                  {
                                    panelClass: 'confirm-sheet-pane', data: res,
                                  }
                                );

                                dialogReference.afterClosed().subscribe({
                                  next: () => {
                                    window.location.reload();
                                  },
                                });
                              } else {
                                window.location.reload();
                              }
                              break;
                          }
                        }
                      },
                      error: (err) => {
                        this.invalidQrCode = true;
                        this.isValidQrScanned = false;
                        this.startCamera();
                      },
                    });
                } else {
                  window.location.reload();
                }
              });
          }
        }
      }

      requestAnimationFrame(this.tick.bind(this));
    } else {
      setTimeout(this.tick.bind(this), 10);
    }
  }

  public isQrCodeValid(data: string): boolean {
    let qrReader: IQrCode;
    try {
      qrReader = JSON.parse(data) as IQrCode;
    } catch {
      return false;
    }

    if (
      qrReader &&
      qrReader.Id !== 0 &&
      qrReader.Name &&
      qrReader.Name.trim() !== '' &&
      Object.values(QrCodeType).includes(qrReader.Type)
    ) {
      return true;
    }

    return false;
  }

  private setLocalStorageSuccessMessage(message: string): void {
    this.afterScanSuccessfulMessage = message;
    localStorage.setItem(
      this.KEY_AFTER_SCAN_SUCCESS_MESSAGE,
      this.afterScanSuccessfulMessage
    );
  }

  private setLocalStorageErrorMessage(message: string): void {
    this.afterScanErrorMessage = message;
    localStorage.setItem(
      this.KEY_AFTER_SCAN_ERROR_MESSAGE,
      this.afterScanErrorMessage
    );
  }

  private initAfterScanSuccessMessage(): void {
    const storedMessage = localStorage.getItem(
      this.KEY_AFTER_SCAN_SUCCESS_MESSAGE
    );
    if (storedMessage) {
      this.afterScanSuccessfulMessage = storedMessage;
      localStorage.removeItem(this.KEY_AFTER_SCAN_SUCCESS_MESSAGE);
    }
  }

  public initAfterScanErrorMessage(): void {
    const storedMessage = localStorage.getItem(
      this.KEY_AFTER_SCAN_ERROR_MESSAGE
    );
    if (storedMessage) {
      this.afterScanErrorMessage = storedMessage;
      this.invalidQrCode = true;
      localStorage.removeItem(this.KEY_AFTER_SCAN_ERROR_MESSAGE);
    }
  }
}
