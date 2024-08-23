import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { UntypedFormGroup, AbstractControl } from '@angular/forms';
import memoizeOne from 'memoize-one';
import { ToastType } from '../../models/toast.model';
import { ToastService } from '../../services/toast.service';

interface IValidationError {
  [key: string]: any;
}

@Component({
  template: '',
})
export class Form {
  public form!: UntypedFormGroup;
  public getFirstErrorMessage: string | null = null;

  constructor(public toastService: ToastService) {}

  public getFieldByName = memoizeOne(this.getFieldByNameUnmemoized);
  public handleErrors = memoizeOne(this.handleErrorsUnmemoized);

  private getFieldByNameUnmemoized(name: string): AbstractControl | null {
    return this.form.get(name);
  }

  private handleErrorsUnmemoized(error: HttpErrorResponse): boolean {
    if (error.status === HttpStatusCode.UnprocessableEntity) {
      return this.setValidationError(error.error.errors as IValidationError);
    } else if (error.status === HttpStatusCode.InternalServerError) {
      this.toastService.error({
        message: 'Something wrong!',
        type: ToastType.Error,
      });
    }
    return false;
  }

  private setValidationError(errors: IValidationError): boolean {
    this.getFirstErrorMessage = null; 

    if (errors) {
      const errorKeys = Object.keys(errors);
      const controlNames = Object.keys(this.form.controls);
      for (const errorKey of errorKeys) {
        for (const controlName of controlNames) {
          if (errorKey.toLowerCase() === controlName.toLowerCase()) {
            const controlErrors = errors[errorKey];
            if (!this.getFirstErrorMessage && controlErrors.length > 0) {
              this.getFirstErrorMessage = controlErrors[0];
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
