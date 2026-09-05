import { Directive, ElementRef, HostListener } from '@angular/core';

/**
 * Native date/time inputs are styled with `appearance: none` globally
 * (see styles.scss) to fix iOS Safari sizing - but that also makes the
 * browser's own calendar/clock icon stop responding to clicks in some
 * WebViews (notably the Android app shell). Forcing `showPicker()` on
 * click restores it everywhere the icon (or the rest of the field) is
 * tapped, without touching the native segment-editing behavior.
 */
@Directive({
  selector:
    'input[type="date"], input[type="time"], input[type="datetime-local"], input[type="month"], input[type="week"]',
  standalone: true,
})
export class ShowPickerOnClickDirective {
  constructor(private readonly el: ElementRef<HTMLInputElement>) {}

  @HostListener('click')
  showPicker(): void {
    const input = this.el.nativeElement as HTMLInputElement & {
      showPicker?: () => void;
    };
    if (typeof input.showPicker === 'function') {
      try {
        input.showPicker();
      } catch {
        // Native date/time segments already handle the tap; nothing else to do.
      }
    }
  }
}
