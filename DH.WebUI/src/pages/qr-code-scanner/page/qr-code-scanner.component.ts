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
  private scanning = false;
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
    this.scanning = false;
    this.stopCamera();
  }

  private stopCamera(): void {
    this.scanning = false;
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
      .getUserMedia({
        video: {
          facingMode: 'environment',
          width: { ideal: 1280 },
          height: { ideal: 720 },
        },
      })
      .then(async (stream) => {
        this.mediaStream = stream;

        const video = this.videoElement.nativeElement;
        video.srcObject = stream;
        // Android WebView / iOS: without playsinline + muted the element goes
        // fullscreen or play() is rejected by the autoplay policy, which looked
        // like the "zoom in/out" jitter.
        video.setAttribute('playsinline', 'true');
        video.setAttribute('webkit-playsinline', 'true');
        video.muted = true;

        try {
          await video.play();
        } catch (err) {
          this.afterScanErrorMessage = this.translateService.instant(
            'qr_scanner.camera_unavailable'
          );
          console.log(err);
          return;
        }

        // Size the decode canvas to the real frame - it defaults to 300x150,
        // which squashes the QR beyond what jsQR can read.
        this.syncCanvasToVideo();

        this.scanning = true;
        requestAnimationFrame(this.tick.bind(this));
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

  private syncCanvasToVideo(): void {
    const video = this.videoElement?.nativeElement;
    if (!video || !video.videoWidth || !video.videoHeight) return;
    if (
      this.canvas.width !== video.videoWidth ||
      this.canvas.height !== video.videoHeight
    ) {
      this.canvas.width = video.videoWidth;
      this.canvas.height = video.videoHeight;
    }
  }

  private tick(): void {
    if (!this.scanning || !this.videoElement) {
      return;
    }
    const video = this.videoElement.nativeElement;

    if (video.readyState === video.HAVE_ENOUGH_DATA) {
      this.syncCanvasToVideo();

      if (!this.canvas.width || !this.canvas.height) {
        requestAnimationFrame(this.tick.bind(this));
        return;
      }

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
        const code = jsQR(imageData.data, imageData.width, imageData.height, {
          inversionAttempts: 'dontInvert',
        });

        if (code) {
          this.afterScanSuccessfulMessage = null;
          video.pause();

          const scanned = code.data;
          let qrType: QrCodeType | null = null;

          if (/^[1-6][0-9A-Z]{11}$/.test(scanned)) {
            // New short token - the leading digit is the type; the server
            // resolves the rest. No client-side decrypt.
            qrType = Number(scanned[0]) as QrCodeType;
          } else {
            // Legacy encrypted-JSON blob (QR codes printed before the token scheme).
            let decrypted: string | null = null;
            try {
              decrypted = this.qrEncryptService.decryptObjectSync(scanned);
            } catch {
              decrypted = null;
            }
            if (decrypted && this.isQrCodeValid(decrypted)) {
              qrType = (JSON.parse(decrypted) as IQrCode).Type;
            }
          }

          if (qrType === null) {
            this.invalidQrCode = true;
            video.play();
            setTimeout(() => {
              this.invalidQrCode = false;
            }, 3000);
          } else {
            this.currentQrCodeType = qrType;
            this.invalidQrCode = false;
            const request = { data: scanned };
            this.isValidQrScanned = true;
            this.scanning = false; // stop the rAF loop; the video is now hidden

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
