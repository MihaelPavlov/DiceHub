import {
  AfterViewInit,
  Component,
  ElementRef,
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

@Component({
  selector: 'app-qr-code-scanner',
  templateUrl: 'qr-code-scanner.component.html',
  styleUrl: 'qr-code-scanner.component.scss',
})
export class QrCodeScannerComponent implements OnInit, AfterViewInit {
  @ViewChild('video') videoElement!: ElementRef<HTMLVideoElement>;
  private currentStream: MediaStream | null = null;
  private readonly KEY_AFTER_SCAN_SUCCESS_MESSAGE = 'afterScanSuccessMessage';
  public imageSrc: string | null = null;
  public canvas!: HTMLCanvasElement;
  public context!: CanvasRenderingContext2D | null;
  public invalidQrCode = false;
  public isValidQrScanned = false;
  public afterScanSuccessfulMessage: string | null = null;

  constructor(
    private readonly scannerService: ScannerService,
    private readonly router: Router,
    private readonly dialog: MatDialog
  ) {}

  public ngOnInit(): void {
    this.initAfterScanSuccessMessage();
  }

  public ngAfterViewInit(): void {
    this.canvas = document.createElement('canvas');
    this.context = this.canvas.getContext('2d');
    this.startCamera();
  }

  startCamera() {
    navigator.mediaDevices
      .getUserMedia({ video: { facingMode: 'environment' } })
      .then((stream) => {
        try {
          this.videoElement.nativeElement.srcObject = stream;
          this.videoElement.nativeElement.play();
          requestAnimationFrame(this.tick.bind(this));
        } catch (err) {
          alert(err);
        }
      })
      .catch((err) => {
        alert(err);
        console.log(err);
      });
  }

  tick() {
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
          console.log('QR Code detected:', code.data);
          this.afterScanSuccessfulMessage = null;
          video.pause();
          if (!this.isQrCodeValid(code.data)) {
            console.log('Invalid QR Code detected:', code.data);

            this.invalidQrCode = true;
            video.play();
          } else {
            this.invalidQrCode = false;
            const request = { data: code.data };
            this.isValidQrScanned = true;

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
                            `space/create/${res.objectId}`
                          );
                        }
                        break;

                      case QrCodeType.GameReservation:
                        if (res.isValid) {
                          this.setLocalStorageSuccessMessage(
                            `Game Reservation is VALID \n\n ${res.internalNote ?? 'No Note From Staff'}`
                          );
                          window.location.reload();
                        } else {
                          const dialogRef = this.dialog.open(
                            ScanResultAdminDialog,
                            {
                              data: res,
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
                            `Table Reservation is VALID \n\n ${res.internalNote ?? 'No Note From Staff'}`
                          );
                          window.location.reload();
                        } else {
                          const dialogRef = this.dialog.open(
                            ScanResultAdminDialog,
                            {
                              data: res,
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
                            data: res,
                          }
                        );

                        dialogRef.afterClosed().subscribe({
                          next: () => {
                            window.location.reload();
                          },
                        });
                        break;
                    }
                  }
                  // video.remove();
                },
                error: (err) => {
                  console.log(err, 'error');

                  this.invalidQrCode = true;
                  this.isValidQrScanned = false;
                  this.startCamera();
                },
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
    console.log(data);

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

  private initAfterScanSuccessMessage(): void {
    const storedMessage = localStorage.getItem(
      this.KEY_AFTER_SCAN_SUCCESS_MESSAGE
    );
    if (storedMessage) {
      this.afterScanSuccessfulMessage = storedMessage;
      localStorage.removeItem(this.KEY_AFTER_SCAN_SUCCESS_MESSAGE);
    }
  }
}
