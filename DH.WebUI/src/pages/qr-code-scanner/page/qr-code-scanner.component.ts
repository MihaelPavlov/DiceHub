import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import jsQR from 'jsqr';
import { ScannerService } from '../../../entities/qr-code-scanner/api/scanner.service';
import { IQrCode } from '../../../entities/qr-code-scanner/models/qr-code.model';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { take } from 'rxjs';
import { Router } from '@angular/router';
import { IQrCodeValidationResult } from '../../../entities/qr-code-scanner/models/qr-code-validation-result.model';

@Component({
  selector: 'app-qr-code-scanner',
  templateUrl: 'qr-code-scanner.component.html',
  styleUrl: 'qr-code-scanner.component.scss',
})
export class QrCodeScannerComponent implements AfterViewInit {
  @ViewChild('video') videoElement!: ElementRef<HTMLVideoElement>;
  // @ViewChild('image') imageElement!: ElementRef<HTMLImageElement>;

  public imageSrc: string | null = null;
  public canvas!: HTMLCanvasElement;
  public context!: CanvasRenderingContext2D | null;
  public invalidQrCode = false;
  public isValidQrScanned = false;

  constructor(
    private readonly scannerService: ScannerService,
    private readonly router: Router
  ) {}

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
                  if (res?.isValid) {
                    switch (res.type) {
                      case QrCodeType.Game:
                        this.router.navigateByUrl(
                          `space/create/${res.objectId}`
                        );
                        break;
                    }
                  } else {
                    // video.remove();
                  }
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
}
