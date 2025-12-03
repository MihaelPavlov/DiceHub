import { Component, HostListener } from '@angular/core';

@Component({
    selector: 'app-scroll-to-top',
    templateUrl: 'scroll-to-top.component.html',
    styleUrl: 'scroll-to-top.component.scss',
    standalone: false
})
export class ScrollTopComponent {
  private lastScrollPosition = 0; // To track the last scroll position
  private scrollAmount = 0; // To track the total scroll amount
  public showArrow = false; // Flag to control the visibility of the arrow

  // Listen to the window scroll event
  @HostListener('window:scroll', ['$event'])
  onScroll(event: Event): void {
    const currentScrollPosition = window.scrollY; // Current scroll position
    const scrollThreshold = 400; // You can adjust this threshold to your liking

    // If the user scrolls down by more than a certain amount
    if (currentScrollPosition > this.lastScrollPosition) {
      this.scrollAmount += currentScrollPosition - this.lastScrollPosition;
    } else {
      this.scrollAmount -= this.lastScrollPosition - currentScrollPosition;
    }

    // If the user has scrolled down a certain amount, show the arrow
    if (this.scrollAmount > scrollThreshold) {
      this.showArrow = true;
    } else {
      this.showArrow = false;
    }

    // Update the last scroll position for the next event
    this.lastScrollPosition = currentScrollPosition;
  }

  // Scroll to the top when the button is clicked
  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    this.showArrow = false; // Hide the button after clicking
  }
}
