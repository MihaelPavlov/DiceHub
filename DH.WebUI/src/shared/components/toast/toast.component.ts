import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { IToast } from '../../models/toast.model';
import * as snackBar from '@angular/material/snack-bar';

@Component({
  selector: 'app-toast',
  templateUrl: 'toast.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class ToastComponent {
  progress = 100;
  private currentIntervalId!: any;
  constructor(
    @Inject(snackBar.MAT_SNACK_BAR_DATA) public toastData: IToast,
    public snackBarRef: snackBar.MatSnackBarRef<ToastComponent>
  ) {
    this.snackBarRef.afterOpened().subscribe(
      () => {
        const duration =
          this.snackBarRef.containerInstance.snackBarConfig.duration;
        if (duration) this.runProgressBar(duration);
      },
      (error) => console.error(error)
    );
  }
  runProgressBar(duration: number) {
    this.progress = 100;
    const step = 0.02;
    this.cleanProgressBarInterval();
    this.currentIntervalId = setInterval(() => {
      this.progress -= 100 * (step * 3);

      if (this.progress < 0) {
        this.cleanProgressBarInterval();
        this.close();
      }
    }, duration * (step * 3));
  }

  close() {
    clearInterval(this.currentIntervalId);
    this.snackBarRef.dismissWithAction();
  }
  cleanProgressBarInterval() {
    clearInterval(this.currentIntervalId);
    console.log('progress bar interval(...) cleaned!');
  }
  public getIconPathByType(type: string): string {
    if (type) {
      return `../../../shared/assets/images/${type}-toast-icon.svg`;
    }
    return '';
  }
}
