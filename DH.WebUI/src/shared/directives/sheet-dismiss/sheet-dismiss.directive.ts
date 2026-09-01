import {
  Directive,
  ElementRef,
  HostListener,
  OnDestroy,
  Optional,
} from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

/**
 * Swipe-down-to-dismiss for the shared `.confirm-sheet` bottom-sheet dialogs.
 *
 * Put it on the sheet root: `<div class="confirm-sheet" appSheetDismiss>`.
 * The drag only starts from the grab handle (`.confirm-sheet__handle`) or the
 * header (`.confirm-sheet__head`) - never from the scrollable body or an
 * interactive control - so it can't fight list scrolling inside the sheet.
 * Past a distance/velocity threshold it closes the injected MatDialogRef;
 * otherwise it springs back.
 */
@Directive({
  selector: '[appSheetDismiss]',
  standalone: true,
})
export class SheetDismissDirective implements OnDestroy {
  private readonly DISMISS_PX = 110;
  private readonly DISMISS_VELOCITY = 0.55; // px per ms, downward

  private dragging = false;
  private pointerId: number | null = null;
  private startY = 0;
  private lastY = 0;
  private lastT = 0;

  private readonly onMove = (e: PointerEvent) => this.handleMove(e);
  private readonly onUp = (e: PointerEvent) => this.handleUp(e);

  constructor(
    private readonly el: ElementRef<HTMLElement>,
    @Optional() private readonly dialogRef: MatDialogRef<unknown> | null
  ) {}

  @HostListener('pointerdown', ['$event'])
  handleDown(e: PointerEvent): void {
    if (e.pointerType === 'mouse' && e.button !== 0) return;

    const target = e.target as HTMLElement | null;
    if (!target) return;
    if (!target.closest('.confirm-sheet__handle, .confirm-sheet__head')) return;
    if (
      target.closest('button, a, input, textarea, select, [mat-dialog-close]')
    ) {
      return;
    }

    this.dragging = true;
    this.pointerId = e.pointerId;
    this.startY = this.lastY = e.clientY;
    this.lastT = e.timeStamp;

    const host = this.el.nativeElement;
    host.style.transition = 'none';
    host.style.willChange = 'transform';

    window.addEventListener('pointermove', this.onMove, { passive: false });
    window.addEventListener('pointerup', this.onUp);
    window.addEventListener('pointercancel', this.onUp);
  }

  private handleMove(e: PointerEvent): void {
    if (!this.dragging || e.pointerId !== this.pointerId) return;

    const dy = e.clientY - this.startY;
    this.lastY = e.clientY;
    this.lastT = e.timeStamp;

    // resist upward drags so the sheet feels anchored
    const offset = dy < 0 ? dy / 4 : dy;
    e.preventDefault();
    this.el.nativeElement.style.transform = `translateY(${offset}px)`;
    this.setBackdropOpacity(Math.max(0.25, 1 - Math.max(0, dy) / 500));
  }

  private handleUp(e: PointerEvent): void {
    if (!this.dragging) return;
    this.dragging = false;
    this.removeWindowListeners();

    const host = this.el.nativeElement;
    const dy = e.clientY - this.startY;
    const dt = Math.max(1, e.timeStamp - this.lastT);
    const velocity = (e.clientY - this.lastY) / dt;

    if (dy > this.DISMISS_PX || velocity > this.DISMISS_VELOCITY) {
      host.style.transition = 'transform .18s ease-in';
      host.style.transform = 'translateY(110%)';
      this.setBackdropOpacity(0);
      const close = () => this.dialogRef?.close();
      host.addEventListener('transitionend', close, { once: true });
      setTimeout(close, 220);
    } else {
      host.style.transition = 'transform .22s cubic-bezier(.2, .8, .2, 1)';
      host.style.transform = 'translateY(0)';
      this.setBackdropOpacity(null);
      host.addEventListener(
        'transitionend',
        () => {
          host.style.transition = '';
          host.style.willChange = '';
          host.style.transform = '';
        },
        { once: true }
      );
    }
  }

  private setBackdropOpacity(value: number | null): void {
    const backdrops = document.querySelectorAll<HTMLElement>(
      '.cdk-overlay-backdrop-showing'
    );
    const backdrop = backdrops[backdrops.length - 1];
    if (!backdrop) return;
    backdrop.style.opacity = value === null ? '' : String(value);
  }

  private removeWindowListeners(): void {
    window.removeEventListener('pointermove', this.onMove);
    window.removeEventListener('pointerup', this.onUp);
    window.removeEventListener('pointercancel', this.onUp);
  }

  ngOnDestroy(): void {
    this.removeWindowListeners();
  }
}
