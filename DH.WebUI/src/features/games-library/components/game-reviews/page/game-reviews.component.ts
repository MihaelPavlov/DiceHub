import { Component, OnInit } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GameReviewsService } from '../../../../../entities/games/api/game-reviews.service';
import { IGameReviewListResult } from '../../../../../entities/games/models/game-review-list.model';
import { AuthService } from '../../../../../entities/auth-service';

@Component({
  selector: 'app-game-reviews',
  templateUrl: 'game-reviews.component.html',
  styleUrl: 'game-reviews.component.scss',
})
export class GameReviewsComponent implements OnInit {
  public game!: IGameByIdResult;
  public gameReviews!: IGameReviewListResult[];
  public currentUserId!: string;
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

  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly authService: AuthService,
    private readonly gameReviewService: GameReviewsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router
  ) {
    this.authService.userInfo$.subscribe((x) => {
      if (x) {
        console.log(x);
        
        this.currentUserId = x?.id;
      }
    });
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      const gameId = params['id'];
      this.gameService.getById(gameId).subscribe((x) => {
        this.game = x;
        this.fetchGameReviews();
      });
    });
  }

  private onSaveReview(): void {
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

  public toggleReviewTextarea(
    value: boolean,
    withCreate: boolean = false
  ): void {
    if (withCreate) this.onSaveReview();
    else this.showCommentInput.next(value);
  }

  private fetchGameReviews() {
    this.gameReviewService.getList(this.game.id).subscribe((x) => {
      console.log(x);

      if (x) {
        this.gameReviews = x;
      }
      console.log('games reviews ', this.gameReviews);
    });
  }
}
