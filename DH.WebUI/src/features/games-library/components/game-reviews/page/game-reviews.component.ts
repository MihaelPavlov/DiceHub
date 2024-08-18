import { Component, inject, OnInit } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { BehaviorSubject } from 'rxjs';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GameReviewsService } from '../../../../../entities/games/api/game-reviews.service';
import { IGameReviewListResult } from '../../../../../entities/games/models/game-review-list.model';
import { AuthService } from '../../../../../entities/auth/auth.service';
import { IUserInfo } from '../../../../../entities/auth/models/user-info.model';
import { UserRole } from '../../../../../entities/auth/enums/roles.enum';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ToastType } from '../../../../../shared/models/toast.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { GameReviewConfirmDeleteDialog } from '../components/game-review-confirm-delete/game-review-confirm-delete.component';

enum ReviewState {
  create,
  update,
}

@Component({
  selector: 'app-game-reviews',
  templateUrl: 'game-reviews.component.html',
  styleUrl: 'game-reviews.component.scss',
})
export class GameReviewsComponent implements OnInit {
  public game!: IGameByIdResult;
  public isAbleToDeleteEveryReview: boolean = false;
  public gameReviews!: IGameReviewListResult[];
  public userInfo: IUserInfo | null = this.authService.getUser;
  public showCommentInput = new BehaviorSubject<boolean>(false);
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

  public currentReviewState: ReviewState = ReviewState.create;
  public currentReviewIdForUpdate!: number;
  public ReviewState = ReviewState;
  public readonly dialog = inject(MatDialog);

  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
    private readonly gameReviewService: GameReviewsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router
  ) {}
  public openDeleteDialog(id: number): void {
    const dialogRef = this.dialog.open(GameReviewConfirmDeleteDialog, {
      width: '17rem',
      position: { bottom: '80%', left: '2%' },
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchGameReviews();
      }
    });
  }
  public canDeleteReview(reviewUserId: string): boolean {
    if ((this.userInfo?.role as string) === UserRole.SuperAdmin) return true;

    return this.userInfo?.id === reviewUserId;
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      const gameId = params['id'];
      if (gameId)
        this.gameService.getById(gameId).subscribe((x) => {
          this.game = x;
          this.fetchGameReviews();
        });
    });
  }

  private onCreateReview(): void {
    if (this.reviewForm.valid) {
      const reviewValue = this.reviewForm.get('review')?.value;
      this.gameReviewService
        .create({
          gameId: this.game.id,
          review: reviewValue,
        })
        .subscribe((x) => this.fetchGameReviews());
      this.showCommentInput.next(false);
      this.reviewForm.reset();
    }
  }

  public navigateBackToGameList(): void {
    this.router.navigate(['games/library']);
  }

  private onUpdateReview() {
    if (this.reviewForm.valid) {
      const reviewValue = this.reviewForm.get('review')?.value;
      this.gameReviewService
        .update({
          id: this.currentReviewIdForUpdate,
          review: reviewValue,
        })
        .subscribe((x) => {
          this.toastService.success({
            message: 'Succesefully deleted',
            type: ToastType.Success,
          });
          this.fetchGameReviews();
        });
      this.showCommentInput.next(false);
      this.currentReviewState = ReviewState.create;
      this.reviewForm.reset();
    }
  }

  public toggleReviewTextarea(
    value: boolean,
    triggerAction: boolean = false
  ): void {
    if (triggerAction) {
      if (this.currentReviewState === ReviewState.create) this.onCreateReview();
      else if (this.currentReviewState === ReviewState.update)
        this.onUpdateReview();
    } else if (value === false) {
      this.currentReviewState = ReviewState.create;
      this.reviewForm.reset();
    }
    this.showCommentInput.next(value);
  }

  public startUpdatingComment(id: number, review: string) {
    this.reviewForm.patchValue({ review });
    this.currentReviewIdForUpdate = id;
    this.currentReviewState = ReviewState.update;
    this.showCommentInput.next(true);
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth',
    });
  }

  private fetchGameReviews() {
    this.gameReviewService.getList(this.game.id).subscribe((x) => {
      if (x) this.gameReviews = x;
    });
  }
}
