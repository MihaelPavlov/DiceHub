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

@NgModule({
  declarations: [
    StringFormatPipe,
    ToastComponent,
    CalculateRemainingDaysPipe,
    GameImagePipe,
    EventImagePipe,
  ],
  imports: [CommonModule, ReactiveFormsModule, MatProgressBarModule],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    CalculateRemainingDaysPipe,
    StringFormatPipe,
    GameImagePipe,
    EventImagePipe,
  ],
  providers: [
    StringFormatPipe,
    GameImagePipe,
    EventImagePipe,
    CalculateRemainingDaysPipe,
    TOAST_DEFAULT_OPTIONS,
  ],
})
export class SharedModule {}
