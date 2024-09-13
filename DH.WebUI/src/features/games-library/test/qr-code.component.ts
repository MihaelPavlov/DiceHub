import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import jsQR from 'jsqr';
import { GamesService } from '../../../entities/games/api/games.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-qr-code',
  templateUrl: 'qr-code.component.html',
})
export class QrCodeComponent implements AfterViewInit {
  @ViewChild('video') videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('image') imageElement!: ElementRef<HTMLImageElement>;

  imageSrc: string | null = null;
  canvas!: HTMLCanvasElement;
  context!: CanvasRenderingContext2D | null;
  qrCodeString: string = '';
  qrCodeUrl: string | null = null;
  constructor(private readonly gameService: GamesService) {}

  generateQRCode() {
    this.gameService.generateQRCode(this.qrCodeString).subscribe((response) => {
      this.qrCodeUrl = response.url;
      console.log('QR Code generated:', response.url);
    });
  }

  ngAfterViewInit(): void {
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
          video.remove();
          const data = {
            imageData: code.data,
          };

          this.gameService
            .upload(code.data)
            .pipe(take(1))
            .subscribe((res) => {
              console.log('result -> ', res);
              alert('QR Code detected: ' + code.data);
            });
        }
      }

      requestAnimationFrame(this.tick.bind(this));
    } else {
      setTimeout(this.tick.bind(this), 10);
    }
  }

  capture() {
    // Capture logic can go here
  }
}
