import { Component } from '@angular/core';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { GamesService } from '../../../../entities/games/api/games.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';

interface IRewardsForm {
  rewards: FormArray;
}

@Component({
  selector: 'app-admin-challenges-custom-period',
  templateUrl: 'admin-challenges-custom-period.component.html',
  styleUrl: 'admin-challenges-custom-period.component.scss',
})
export class AdminChallengesCustomPeriodComponent extends Form {
  override form: Formify<IRewardsForm>;

  public gameList: IGameDropdownResult[] = [];

  //TODO: If the user visit this page and the isCustomPeriodOn setting is ON.
  // Make a guard for not leave the page until everything is setup or turn off the setting
  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);

    this.fetchGameList();

    this.form = this.initFormGroup();
  }

  get rewardArray(): FormArray {
    return this.form.controls['rewards'] as unknown as FormArray;
  }
  public getFormGroup(formGroup: AbstractControl<any, any>): FormGroup {
    return formGroup as FormGroup;
  }
  public createReward(): FormGroup {
    return this.fb.group({
      selectedGame: new FormControl<number | null>(null, [Validators.required]),
      requiredPoints: new FormControl<number>(0, [Validators.required]),
    });
  }
  public onSave() {
    console.log('form', this.form.value);
  }

  //TODO: Based on the settings for max Rewards, we need to stop if the user try to add more.
  // For example show message that if you want more you need to adjust the global settings
  public addReward(): void {
    this.rewardArray.push(this.createReward());
  }

  public removeReward(index: number): void {
    this.rewardArray.removeAt(index);
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      rewards: this.fb.array([]),
    });
  }

  private fetchGameList(): void {
    this.gameService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
        // if (this.selectedGame) {
        //   const selectGame = this.gameList.find(
        //     (x) => x.id == this.selectedGame?.id
        //   );
        //   if (selectGame) this.selectedGame = selectGame;
        //}
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
}
