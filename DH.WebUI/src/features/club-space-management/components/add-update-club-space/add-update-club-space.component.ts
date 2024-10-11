import { Component } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';

interface ICreateSpaceTable {
  gameId: number;
  name: string;
  maxPeople: number;
  password: string;
}

@Component({
  selector: 'app-add-update-club-space',
  templateUrl: 'add-update-club-space.component.html',
  styleUrl: 'add-update-club-space.component.scss',
})
export class AddUpdateClubSpaceComponent extends Form {
  override form: Formify<ICreateSpaceTable>;

  public showPassword = false;
  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);
    this.form = this.initFormGroup();
  }
  public togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'gameId':
        return 'Scanned Game';
      case 'name':
        return 'Name';
      case 'maxPeople':
        return 'Max People';
      case 'password':
        return 'Password';
      default:
        return controlName;
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      gameId: new FormControl<number | null>(null, [Validators.required]),
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      maxPeople: new FormControl<number>(3, [Validators.required]),
      password: new FormControl<string>(''),
    });
  }
}
