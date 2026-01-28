import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
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
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly translateService: TranslateService,
    private readonly qrEncryptService: QrEncryptService,
  ) {}

  public ngOnInit(): void {
    this.initAfterScanSuccessMessage();
    this.initAfterScanErrorMessage();
  }

  public async ngAfterViewInit(): Promise<void> {
    this.canvas = document.createElement('canvas');
    this.context = this.canvas.getContext('2d');
    await this.startCamera();
  }

  public async ngOnDestroy(): Promise<void> {
    await this.stopCamera();
  }

  private async stopCamera(): Promise<void> {
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach((track) => track.stop());
      this.mediaStream = null;
    }

    if (this.videoElement?.nativeElement) {
      const video = this.videoElement.nativeElement;
      try {
        await video.pause();
      } catch {}
      video.srcObject = null;
    }
  }

  private async startCamera(): Promise<void> {
    const video: HTMLVideoElement = this.videoElement.nativeElement;

    try {
      // Stop previous stream if exists
      if (this.mediaStream) {
        this.mediaStream.getTracks().forEach((track) => track.stop());
        this.mediaStream = null;
      }

      if (video.srcObject) {
        video.pause();
        video.srcObject = null;
      }

      // Small delay to let iOS fully release camera
      await new Promise((r) => setTimeout(r, 150));

      // Try preferred camera (rear)
      let constraints: MediaStreamConstraints = {
        video: { facingMode: { ideal: 'environment' } },
        audio: false,
      };

      let stream: MediaStream;
      try {
        stream = await navigator.mediaDevices.getUserMedia(constraints);
      } catch (err) {
        console.warn('Preferred camera failed, trying any camera...', err);
        // Fallback to any camera
        stream = await navigator.mediaDevices.getUserMedia({
          video: true,
          audio: false,
        });
      }

      this.mediaStream = stream;

      // Setup video element
      video.srcObject = stream;
      video.muted = true;
      video.setAttribute('playsinline', 'true'); // HTML attribute
      video.playsInline = true; // JS property for iOS

      // Wait until metadata loaded, then play
      video.onloadedmetadata = async () => {
        try {
          await video.play();
          requestAnimationFrame(this.tick.bind(this));
        } catch (playErr) {
          console.warn('Video play failed:', playErr);
        }
      };
    } catch (finalErr) {
      console.error('Camera completely failed:', finalErr);
      alert('Camera could not start on this device.');
    }
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
        this.canvas.height,
      );
      const imageData = this.context?.getImageData(
        0,
        0,
        this.canvas.width,
        this.canvas.height,
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
                width: '17rem',
                data: {
                  type: this.currentQrCodeType,
                },
              },
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
                                this.router.navigateByUrl(
                                  FULL_ROUTE.SPACE_MANAGEMENT.CREATE(
                                    res.objectId,
                                  ),
                                );
                              } else {
                                this.setLocalStorageErrorMessage(
                                  res.errorMessage,
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
                                          'qr_scanner.no_staff_note',
                                        ),
                                    },
                                  ),
                                );
                                window.location.reload();
                              } else {
                                const dialogRef = this.dialog.open(
                                  ScanResultAdminDialog,
                                  {
                                    data: res,
                                  },
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
                                          'qr_scanner.no_staff_note',
                                        ),
                                    },
                                  ),
                                );
                                window.location.reload();
                              } else {
                                const dialogRef = this.dialog.open(
                                  ScanResultAdminDialog,
                                  {
                                    data: res,
                                  },
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
                                  data: res,
                                },
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
                                    data: res,
                                  },
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
      this.afterScanSuccessfulMessage,
    );
  }

  private setLocalStorageErrorMessage(message: string): void {
    this.afterScanErrorMessage = message;
    localStorage.setItem(
      this.KEY_AFTER_SCAN_ERROR_MESSAGE,
      this.afterScanErrorMessage,
    );
  }

  private initAfterScanSuccessMessage(): void {
    const storedMessage = localStorage.getItem(
      this.KEY_AFTER_SCAN_SUCCESS_MESSAGE,
    );
    if (storedMessage) {
      this.afterScanSuccessfulMessage = storedMessage;
      localStorage.removeItem(this.KEY_AFTER_SCAN_SUCCESS_MESSAGE);
    }
  }

  public initAfterScanErrorMessage(): void {
    const storedMessage = localStorage.getItem(
      this.KEY_AFTER_SCAN_ERROR_MESSAGE,
    );
    if (storedMessage) {
      this.afterScanErrorMessage = storedMessage;
      this.invalidQrCode = true;
      localStorage.removeItem(this.KEY_AFTER_SCAN_ERROR_MESSAGE);
    }
  }
}
