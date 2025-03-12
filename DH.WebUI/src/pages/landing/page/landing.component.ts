import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';

@Component({
  selector: 'app-landing',
  templateUrl: 'landing.component.html',
  styleUrl: 'landing.component.scss',
})
export class LandingComponent {
  public slides = [
    { id: 1, image: 1 },
    { id: 2, image: 2 },
    { id: 3, image: 3 },
    { id: 4, image: 4 },
  ];

  public currentSlideIndex = 0;
  public scrollTimeout: any;
  public touchStartX = 0;
  public touchEndX = 0;

  @ViewChild('carouselViewport', { static: false })
  carouselViewport!: ElementRef;

  public scrollToSlide(index: number) {
    if (index < 0 || index >= this.slides.length) return; // Prevent out-of-bounds scrolling

    this.currentSlideIndex = index;
    const viewport = this.carouselViewport.nativeElement;
    const slideWidth = viewport.clientWidth;
    viewport.scrollTo({ left: index * slideWidth, behavior: 'smooth' });
  }

  // Mouse Wheel Scroll (Desktop)
  @HostListener('wheel', ['$event'])
  public onScroll(event: WheelEvent) {
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
    this.handleSwipe(); // Handle swipe direction immediately after touch end
  }
  // Detect Swipe Direction
  public handleSwipe() {
    const swipeThreshold = 30; // Optional: Minimum swipe distance for detection (can be small)

    if (this.touchStartX > this.touchEndX + swipeThreshold) {
      // Swipe Left → Move to Next Slide
      this.scrollToSlide(this.currentSlideIndex + 1);
    } else if (this.touchEndX > this.touchStartX + swipeThreshold) {
      // Swipe Right → Move to Previous Slide
      this.scrollToSlide(this.currentSlideIndex - 1);
    }
  }
}
