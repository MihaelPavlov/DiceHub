import { IOwnerResult } from './../../../../entities/profile/models/owner-result.interface';
import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { Router } from '@angular/router';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { ROUTE } from '../../../../shared/configs/route.config';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

interface IOwnerForm {
  email: string;
  clubName: string;
  clubPhoneNumber: string;
}

@Component({
  selector: 'app-owner-details',
  templateUrl: 'owner-details.component.html',
  styleUrl: 'owner-details.component.scss',
})
export class OwnerDetailsComponent extends Form implements OnDestroy {
  override form: Formify<IOwnerForm>;

  public owner: IOwnerResult | null = null;
  public showOwnerForm: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly usersService: UsersService,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);

    console.log(this.showOwnerForm);

    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.form = this.initFormGroup();
    this.fetchOwner();
  }

  public fetchOwner(): void {
    this.usersService.getOwner().subscribe({
      next: (res: IOwnerResult | null) => {
        this.owner = res;
      },
      error: () => {
        this.owner = null;
      },
    });
  }

  public onAdd(): void {
    this.toggleOwnerForm();
  }

  public onAddOwner(): void {
    this.usersService
      .createOwner(
        this.form.controls.email.value,
        this.form.controls.clubPhoneNumber.value,
        this.form.controls.clubName.value
      )
      .subscribe({
        next: () => {
          this.fetchOwner();
          this.toggleOwnerForm();
        },
      });
  }

  public toggleOwnerForm(): void {
    this.showOwnerForm = !this.showOwnerForm;
  }

  public onRemove(): void {
    this.usersService.deleteOwner().subscribe({
      next: () => {
        this.fetchOwner();
      },
    });
  }

  public onBack(): void {
    this.router.navigateByUrl(ROUTE.PROFILE.CORE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      case 'clubPhoneNumber':
        return 'Club Phone Number';
      case 'clubName':
        return 'Club Name';
      default:
        return controlName;
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
      clubName: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(2),
      ]),
      clubPhoneNumber: new FormControl<string>('', [Validators.required]),
    });
  }
}
