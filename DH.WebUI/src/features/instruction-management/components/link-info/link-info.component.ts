import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  Input,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { InstructionStep } from '../../../../entities/instruction-management/models/instruction.model';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../shared/dialogs/image-preview/image-preview.dialog';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-link-info',
    templateUrl: 'link-info.component.html',
    styleUrl: 'link-info.component.scss',
    standalone: false
})
export class LinkInfoComponent implements AfterViewInit {
  @Input({ required: true }) linkInfo!: InstructionStep[];
  public slides: { id: number }[] = [
    // { id: 2, image: 2 },
    // { id: 3, image: 3 },
    // { id: 4, image: 4 },
  ];

  public currentSlideIndex = 0;
  public scrollTimeout: any;
  public touchStartX = 0;
  public touchEndX = 0;
  public currentIndex: number = 0;
  public gifSrcMap: { [index: number]: string } = {};
  @ViewChild('carouselViewport', { static: false })
  carouselViewport!: ElementRef;

  constructor(
    private cd: ChangeDetectorRef,
    private readonly dialog: MatDialog,
    private readonly translateService: TranslateService
  ) {}

  public translate(key: string): string {
    return this.translateService.instant(key);
  }

  public ngAfterViewInit(): void {
    // Ensure snapping after manual scrolls
    if (this.carouselViewport.nativeElement)
      this.carouselViewport.nativeElement.addEventListener(
        'scroll',
        this.onScrollEnd.bind(this)
      );

    setTimeout(() => {
      this.gifSrcMap[this.currentIndex] =
        this.translate(this.linkInfo[this.currentIndex].mediaUrl ?? '') +
        '?t=' +
        Date.now();
    });
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes['linkInfo'] && this.linkInfo?.length) {
      this.slides = Array.from({ length: this.linkInfo.length }, (_, i) => ({
        id: i + 1,
      }));
    }
  }

  public openImagePreview(imageUrl: string) {
    console.log('image preview ->', imageUrl);

    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translateService.instant("image"),
        removeHeight: true,
      },
      width: '17rem',
    });
  }

  public get getCurrentLinkInfo(): InstructionStep {
    return this.linkInfo[this.currentSlideIndex];
  }

  public scrollToSlide(index: number): void {
    if (index < 0 || index >= this.slides.length) return; // Prevent out-of-bounds scrolling

    this.currentSlideIndex = index;
    const viewport = this.carouselViewport.nativeElement;
    const slideWidth = viewport.clientWidth;
    viewport.scrollTo({ left: index * slideWidth, behavior: 'smooth' });
    // Wait 2 seconds before scrolling to top
    window.scrollTo({ top: 0, behavior: 'instant' });

    // Reset the GIF/Image source
    this.currentIndex = index;
    const imagePath = this.translate(this.linkInfo[index].mediaUrl ?? '');
    this.gifSrcMap[index] = `${imagePath}?t=${Date.now()}`;
  }

  // Handle manual scrolling and snap to the nearest slide
  private onScrollEnd(): void {
    if (this.scrollTimeout) {
      clearTimeout(this.scrollTimeout);
    }

    this.scrollTimeout = setTimeout(() => {
      const viewport = this.carouselViewport.nativeElement;
      const slideWidth = viewport.clientWidth;
      const nearestIndex = Math.round(viewport.scrollLeft / slideWidth);

      this.scrollToSlide(nearestIndex);
    }, 150);
  }

  // Mouse Wheel Scroll (Desktop)
  @HostListener('wheel', ['$event'])
  public onMouseWheel(event: WheelEvent) {
    event.preventDefault();

    if (this.scrollTimeout) {
      clearTimeout(this.scrollTimeout);
    }

    this.scrollTimeout = setTimeout(() => {
      if (event.deltaY > 0) {
        this.scrollToSlide(this.currentSlideIndex + 1);
      } else {
        this.scrollToSlide(this.currentSlideIndex - 1);
      }
    }, 100);
  }

  // Touch Start (Mobile)
  @HostListener('touchstart', ['$event'])
  public onTouchStart(event: TouchEvent) {
    this.touchStartX = event.touches[0].clientX;
  }

  // Touch End (Mobile)
  @HostListener('touchend', ['$event'])
  public onTouchEnd(event: TouchEvent) {
    this.touchEndX = event.changedTouches[0].clientX;
    this.handleSwipe();
  }

  // Detect Swipe Direction
  public handleSwipe(): void {
    const swipeThreshold = 100; // Minimum swipe distance for detection

    if (this.touchStartX > this.touchEndX + swipeThreshold) {
      this.scrollToSlide(this.currentSlideIndex + 1); // Swipe Left → Next Slide
    } else if (this.touchEndX > this.touchStartX + swipeThreshold) {
      this.scrollToSlide(this.currentSlideIndex - 1); // Swipe Right → Previous Slide
    }
  }
}
