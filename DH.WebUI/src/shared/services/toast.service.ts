import { Inject, Injectable } from '@angular/core';
import {
  MAT_SNACK_BAR_DEFAULT_OPTIONS,
  MatSnackBar,
  MatSnackBarConfig,
} from '@angular/material/snack-bar';
import { IToast, TOAST_CLASS } from '../models/toast.model';
import { ToastComponent } from '../components/toast/toast.component';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly snackBarRef: any;

  constructor(
    public toast: MatSnackBar,
    @Inject(MAT_SNACK_BAR_DEFAULT_OPTIONS)
    private defaultSnackBarOptions: MatSnackBarConfig
  ) {}

  public success(data: IToast): void {
    this.toast.openFromComponent(ToastComponent, {
      data,
      duration: data.duration ?? this.defaultSnackBarOptions.duration,
      panelClass: TOAST_CLASS.SUCCESS,
    });
  }

  public error(data: IToast): void {
    this.toast.openFromComponent(ToastComponent, {
      data,
      duration: data.duration ?? this.defaultSnackBarOptions.duration,
      panelClass: TOAST_CLASS.ERROR,
    });
  }

  public closeSnackbar = () => {
    this.snackBarRef.dismiss();
  };
}
