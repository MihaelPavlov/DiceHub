import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
import { NavigationService } from '../../../../shared/services/navigation-service';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { throwError } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

interface IEmployeeForm {
  email: string;
  lastName: string;
  firstName: string;
  phoneNumber: string;
}

@Component({
    selector: 'app-add-update-employee',
    templateUrl: 'add-update-employee.component.html',
    styleUrl: 'add-update-employee.component.scss',
    standalone: false
})
export class AddUpdateEmployeeComponent extends Form implements OnDestroy {
  override form: Formify<IEmployeeForm>;
  public isMenuVisible: boolean = false;
  public editEmployeeId: string | null = null;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    public override readonly toastService: ToastService,
    private readonly usersService: UsersService,
    private readonly navigationService: NavigationService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly fb: FormBuilder,
    private readonly router: Router,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    this.activatedRoute.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.editEmployeeId = id;
        this.fetchEmployeeById(this.editEmployeeId);
      }
    });
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
          this.form.controls.email.value,
          this.form.controls.phoneNumber.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
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
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public onUpdate(): void {
    if (this.form.valid) {
      this.usersService
        .updateEmployee(
          this.editEmployeeId as string,
          this.form.controls.firstName.value,
          this.form.controls.lastName.value,
          this.form.controls.email.value,
          this.form.controls.phoneNumber.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
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
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });
          },
        });
    }
  }

  private fetchEmployeeById(employeeId: string): void {
    this.usersService.getEmployeeById(employeeId).subscribe({
      next: (employee) => {
        if (employee) {
          this.form.patchValue({
            email: employee.email,
            firstName: employee.firstName,
            lastName: employee.lastName,
            phoneNumber: employee.phoneNumber,
          });
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
  }

  public onBack(): void {
    let url = this.navigationService.getPreviousUrl();

    if (url) this.router.navigateByUrl(url);
    else this.router.navigateByUrl(FULL_ROUTE.PROFILE.EMPLOYEES);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return this.translateService.instant(
          'profile.add_update_employee.control_display_names.email'
        );
      case 'firstName':
        return this.translateService.instant(
          'profile.add_update_employee.control_display_names.first_name'
        );
      case 'lastName':
        return this.translateService.instant(
          'profile.add_update_employee.control_display_names.last_name'
        );
      case 'phoneNumber':
        return this.translateService.instant(
          'profile.add_update_employee.control_display_names.phone_number'
        );
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
      phoneNumber: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
    });
  }
}
