import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { UntypedFormGroup, AbstractControl } from '@angular/forms';
import memoizeOne from 'memoize-one';
import { ToastType } from '../../models/toast.model';
import { ToastService } from '../../services/toast.service';
import { AppToastMessage } from '../toast/constants/app-toast-messages.constant';

interface IValidationError {
  [key: string]: any;
}

@Component({
  template: '',
})
export abstract class Form {
  public form!: UntypedFormGroup;
  public getServerErrorMessage: string | null = null;
  constructor(public toastService: ToastService) {
  }

  public getFieldByName = memoizeOne(this.getFieldByNameUnmemoized);

  public getFirstErrorMessage(): string | null {
    const controls = this.form.controls;
    for (const controlName in controls) {
      if (controls.hasOwnProperty(controlName)) {
        const errorMessage = this.handleFormErrors(controlName);
        
        if (errorMessage) {
          return errorMessage;
        }
      }
    }

    if (this.getServerErrorMessage) return this.getServerErrorMessage;

    // Check for additional errors if no form control errors are found
    const additionalError = this.handleAdditionalErrors();
    if (additionalError) {
      return additionalError;
    }

    return null;
  }

  protected abstract getControlDisplayName(controlName: string): string;

  protected handleAdditionalErrors(): string | null {
    return null; // Can be overridden by subclasses to provide additional error handling
  }

  private getFieldByNameUnmemoized(name: string): AbstractControl | null {
    return this.form.get(name);
  }

  public handleServerErrors(error: HttpErrorResponse): boolean {
    if (error.status === HttpStatusCode.UnprocessableEntity) {
      return this.setValidationError(error.error.errors as IValidationError);
    } else if (error.status === HttpStatusCode.InternalServerError) {
      this.toastService.error({
        message: AppToastMessage.SomethingWrong,
        type: ToastType.Error,
      });
    }
    return false;
  }

  private handleFormErrors(controlName: string): string {
    const control = this.form.get(controlName);
    
    if (control && control.errors && (control.touched || control.dirty)) {
      if (control.errors['required']) {
        return `${this.getControlDisplayName(controlName)} is required.`;
      } else if (control.errors['minlength']) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `${this.getControlDisplayName(
          controlName
        )} must be at least ${requiredLength} characters long.`;
      } else if (control.errors['maxlength']) {
        const requiredLength = control.errors['maxlength'].requiredLength;
        return `${this.getControlDisplayName(
          controlName
        )} cannot exceed ${requiredLength} characters.`;
      } else if (control.errors['min']) {
        return `${this.getControlDisplayName(controlName)} must be at least ${
          control.errors['min'].min
        }.`;
      }
    }

    return '';
  }

  private setValidationError(errors: IValidationError): boolean {
    this.getServerErrorMessage = null;

    if (errors) {
      const errorKeys = Object.keys(errors);
      const controlNames = Object.keys(this.form.controls);
      for (const errorKey of errorKeys) {
        for (const controlName of controlNames) {
          if (errorKey.toLowerCase() === controlName.toLowerCase()) {
            const controlErrors = errors[errorKey];
            if (!this.getServerErrorMessage && controlErrors.length > 0) {
              this.getServerErrorMessage = controlErrors[0];
            }

            // How to use it in the template

            /*
              <mat-error
                class="control__error"
                *ngIf="getFieldByName('lastName')?.errors?.['required']"
              >
                Field is required
              </mat-error>
              */
            this.form.get(controlName)?.setErrors({
              isNotUnique: controlErrors[0].includes('unique'),
              required: controlErrors[0].includes('required'),
              invalid: controlErrors[0].includes('invalid'),
              message: controlErrors[0],
            });
          }
        }
      }
    }

    return !!errors;
  }
}
