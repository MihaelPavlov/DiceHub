import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';

@Component({
  selector: 'app-add-update-game',
  templateUrl: 'add-update-game.component.html',
  styleUrl: 'add-update-game.component.scss',
})
export class AddUpdateGameComponent implements OnInit {
  public reviewForm: FormGroup = this.fb.group({
    review: [
      '',
      [
        Validators.required,
        Validators.maxLength(100),
        Validators.minLength(10),
      ],
    ],
  });
  public imagePreview: string | ArrayBuffer | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  public backNavigateBtn(){
    this.router.navigateByUrl('games/library')
  }
  public onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }
}
