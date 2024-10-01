import { Component } from '@angular/core';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { IGameListResult } from '../../../../entities/games/models/game-list.model';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { GamesService } from '../../../../entities/games/api/games.service';
import { Form } from '../../../../shared/components/form/form.component';
import { ToastService } from '../../../../shared/services/toast.service';
import { Location } from '@angular/common';
import { Formify } from '../../../../shared/models/form.model';
interface IChallengeForm {
  selectedGame: number;
  rewardPoints: number;
  attempts: number;
  type: number;
  description: string;
}
@Component({
  selector: 'app-admin-challenges-list',
  templateUrl: 'admin-challenges-list.component.html',
  styleUrl: 'admin-challenges-list.component.scss',
})
export class AdminChallengesListComponent extends Form {
  override form: Formify<IChallengeForm>;

  public gameList: IGameDropdownResult[] = [];
  public games: IGameListResult[] = [];
  public showAddChallenge: boolean = false;
  public showFilter: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly fb: FormBuilder,
    private readonly location: Location
  ) {
    super(toastService);

    this.fetchGameList();
    this.gameService.getList('').subscribe({
      next: (gameList) => (this.games = gameList ?? []),
      error: (error) => {
        console.log(error);
      },
    });
    this.form = this.initFormGroup();
  }

  public toggleFilter(): void {
    this.showFilter = !this.showFilter;
  }

  public toggleAddChallenge() {
    this.showAddChallenge = !this.showAddChallenge;
  }

  public onHideChallenge(): void {
    this.showAddChallenge = false;
  }

  public backNavigateBtn() {
    this.location.back();
  }

  public onAddChallenge() {
    console.log('form', this.form.value);
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
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

  private initFormGroup(): FormGroup {
    return this.fb.group({
      selectedGame: new FormControl<number | null>(null, [Validators.required]),
      rewardPoints: new FormControl<number>(0, [Validators.required]),
      attempts: new FormControl<number>(0, [Validators.required]),
      type: new FormControl<number | null>(null, [Validators.required]),
      description: new FormControl<string>('', [Validators.required]),
    });
  }
}
