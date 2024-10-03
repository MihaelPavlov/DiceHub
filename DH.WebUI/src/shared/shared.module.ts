import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { StringFormatPipe } from './pipe/string-format.pipe';
import { TOAST_DEFAULT_OPTIONS } from './models/toast.model';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ToastComponent } from './components/toast/toast.component';
import { CalculateRemainingDaysPipe } from './pipe/calculate-remaining-days.pipe';
import { GameImagePipe } from './pipe/game-image.pipe';
import { EventImagePipe } from './pipe/event-image.pipe';
import { RewardImagePipe } from './pipe/reward-image.pipe';

@NgModule({
  declarations: [
    StringFormatPipe,
    ToastComponent,
    CalculateRemainingDaysPipe,
    GameImagePipe,
    RewardImagePipe,
    EventImagePipe,
  ],
  imports: [CommonModule, ReactiveFormsModule, MatProgressBarModule],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    CalculateRemainingDaysPipe,
    StringFormatPipe,
    GameImagePipe,
    RewardImagePipe,
    EventImagePipe,
  ],
  providers: [
    StringFormatPipe,
    GameImagePipe,
    RewardImagePipe,
    EventImagePipe,
    CalculateRemainingDaysPipe,
    TOAST_DEFAULT_OPTIONS,
  ],
})
export class SharedModule {}
