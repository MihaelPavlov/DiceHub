import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

interface IEmployeeForm {
  email: string;
  lastName: string;
  firstName: string;
}

@Component({
  selector: 'app-profile',
  templateUrl: 'add-employee.component.html',
  styleUrl: 'add-employee.component.scss',
})
export class AddEmployeeComponent extends Form implements OnDestroy {
  override form: Formify<IEmployeeForm>;
  public isMenuVisible: boolean = false;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    public override readonly toastService: ToastService,
    private readonly usersService: UsersService,
    private readonly fb: FormBuilder,
    private readonly router: Router
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onAdd(): void {
    if (this.form.valid) {
      this.usersService
        .createEmployee(
          this.form.controls.firstName.value,
          this.form.controls.lastName.value,
          this.form.controls.email.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.onBack();
          },
          error: (error) => {
            if (error.error?.errors?.Exist) {
              this.getServerErrorMessage = error.error.errors.Exist[0];
            } else {
              this.handleServerErrors(error);
            }
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public onBack(): void {
    this.router.navigateByUrl('profile/employees');
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      case 'firstName':
        return 'First Name';
      case 'lastName':
        return 'Last Name';
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      firstName: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      lastName: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
    });
  }
}
