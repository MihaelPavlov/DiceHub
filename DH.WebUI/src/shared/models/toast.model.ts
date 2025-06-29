import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';

export interface IToast {
  message: string;
  type: ToastType;
  data?: any;
  title?: string;
  duration?: number;
}

export const TOAST_DEFAULT_OPTIONS = {
  provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
  useValue: {
    horizontalPosition: 'center',
    verticalPosition: 'bottom',
    duration: 4000,
  },
};

export const TOAST_CLASS = {
  INFO: 'toast-info',
  SUCCESS: 'toast-success',
  ERROR: 'toast-error',
  WARNING: 'toast-warning',
  CUSTOM: 'toast-custom',
};

export enum ToastType {
  Success = 'success',
  Error = 'error',
}
