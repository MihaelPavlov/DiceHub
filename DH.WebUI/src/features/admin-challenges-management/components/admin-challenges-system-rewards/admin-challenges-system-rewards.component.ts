import { Component } from '@angular/core';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { GamesService } from '../../../../entities/games/api/games.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Location } from '@angular/common';
import { IGameListResult } from '../../../../entities/games/models/game-list.model';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { SearchService } from '../../../../shared/services/search.service';

interface ISystemRewardsForm {
  selectedLevel: number;
  requiredPoints: number;
  name: string;
  description: string;
  image: string;
}

@Component({
  selector: 'app-admin-challenges-system-rewards',
  templateUrl: 'admin-challenges-system-rewards.component.html',
  styleUrl: 'admin-challenges-system-rewards.component.scss',
})
export class AdminChallengesSystemRewardsComponent extends Form {
  override form: Formify<ISystemRewardsForm>;

  public gameList: IGameDropdownResult[] = [];
  public isMenuVisible: boolean = false;
  public imagePreview: string | ArrayBuffer | null = null;
  public fileToUpload: File | null = null;
  public imageError: string | null = null;
  public games: IGameListResult[] = [];
  public showAddReward: boolean = false;

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

  public handleSearchExpression(searchExpression: string) {}
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public getFormGroup(formGroup: AbstractControl<any, any>): FormGroup {
    return formGroup as FormGroup;
  }
  
  public toggleAddReward() {
    this.showAddReward = !this.showAddReward;
  }

  public onAddReward() {
    console.log('form', this.form.value);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    console.log(input.files);

    const file = input.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.form.controls.image.patchValue(file.name);
        this.fileToUpload = file;
        this.imageError = null;
        console.log(this.form.controls);
      };
      reader.readAsDataURL(file);
    } else {
      this.imageError = 'Image is required.';
      this.fileToUpload = null;
      this.imagePreview = null;
      this.form.controls.image.reset();
    }
  }

  public onHideReward(): void {
    this.showAddReward = false;
  }

  public backNavigateBtn() {
    this.location.back();
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      selectedLevel: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      requiredPoints: new FormControl<number>(0, [Validators.required]),
      name: new FormControl<string>('', [Validators.required]),

      description: new FormControl<string>('', [Validators.required]),
      image: new FormControl<string | null>('', [Validators.required]),
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
