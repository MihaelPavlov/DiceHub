import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { IToast, TOAST_CLASS } from '../models/toast.model';
import { ToastComponent } from '../components/toast/toast.component';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly snackBarRef: any;

  constructor(public toast: MatSnackBar) {}

  public success(data: IToast): void {
    this.toast.openFromComponent(ToastComponent, {
      data,
      panelClass: TOAST_CLASS.SUCCESS,
    });
  }

  public error(data: IToast): void {
    this.toast.openFromComponent(ToastComponent, {
      data,
      panelClass: TOAST_CLASS.ERROR,
    });
    console.error(`Error: ${data.message}`, data || '');
  }

  public closeSnackbar = () => {
    this.snackBarRef.dismiss();
  };
}
