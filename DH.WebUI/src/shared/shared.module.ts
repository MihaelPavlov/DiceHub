import { CommonModule, DatePipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TOAST_DEFAULT_OPTIONS } from './models/toast.model';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ToastComponent } from './components/toast/toast.component';
import { CalculateRemainingDaysPipe } from './pipe/calculate-remaining-days.pipe';
import { ControlsMenuComponent } from './components/menu/controls-menu.component';
import { RandomColorDirective } from './directives/random-color/random-color.directive';
import { EntityImagePipe } from './pipe/entity-image.pipe';

@NgModule({
  declarations: [
    ToastComponent,
    CalculateRemainingDaysPipe,
    EntityImagePipe,
    ControlsMenuComponent,
    RandomColorDirective,
  ],
  imports: [CommonModule, ReactiveFormsModule, MatProgressBarModule],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    CalculateRemainingDaysPipe,
    EntityImagePipe,
    ControlsMenuComponent,
    RandomColorDirective,
    DatePipe,
  ],
  providers: [
    EntityImagePipe,
    CalculateRemainingDaysPipe,
    DatePipe,
    TOAST_DEFAULT_OPTIONS,
  ],
})
export class SharedModule {}
