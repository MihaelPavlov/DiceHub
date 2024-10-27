import { Component, OnInit } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { WeekDay } from '@angular/common';
import { Formify } from '../../../../shared/models/form.model';

interface ITenantSettingsForm {
  averageMaxCapacity: number;
  challengeRewardsCountForPeriod: number;
  periodOfRewardReset: string;
  resetDayForRewards: string;
  challengeInitiationDelayHours: number;
}

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'app-global-settings',
  templateUrl: 'global-settings.component.html',
  styleUrl: 'global-settings.component.scss',
})
export class GlobalSettingsComponent extends Form implements OnInit {
    override form: Formify<ITenantSettingsForm>;
    public isMenuVisible: boolean = false;

  public weekDaysValues: IDropdown[] = [];

  constructor(
    private readonly fb: FormBuilder,
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);

    this.weekDaysValues = Object.entries(WeekDay)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));
  }
  public ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

public onSave():void{
    console.log(this.form.controls);
    
}

  public backNavigateBtn() {
    this.router.navigateByUrl('profile');
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      averageMaxCapacity: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      challengeRewardsCountForPeriod: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      periodOfRewardReset: new FormControl<string | null>(
        '',
        Validators.required
      ),
      resetDayForRewards: new FormControl<string | null>(
        'Sunday',
        Validators.required
      ),
      challengeInitiationDelayHours: new FormControl<string | null>('12:00', [
        Validators.required,
      ]),
    });
  }
}
