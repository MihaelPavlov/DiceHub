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
import { TruncatePipe } from './pipe/truncate.pipe';
import { ParseDateTagPipe } from './pipe/parse-date-tag.pipe';
import { PasswordVisibilityToggleComponent } from './components/password-visibility-toggle/password-visibility-toggle.component';
import { TranslateModule, TranslatePipe } from '@ngx-translate/core';
import { TranslateInPipe } from './pipe/translate-in.pipe';
import { InstallPromptComponent } from './components/install-prompt/install-prompt.component';
import { SheetDismissDirective } from './directives/sheet-dismiss/sheet-dismiss.directive';
import { ShowPickerOnClickDirective } from './directives/show-picker-on-click/show-picker-on-click.directive';

@NgModule({
  declarations: [
    ToastComponent,
    CalculateRemainingDaysPipe,
    EntityImagePipe,
    TruncatePipe,
    ParseDateTagPipe,
    TranslateInPipe,
    ControlsMenuComponent,
    RandomColorDirective,
    PasswordVisibilityToggleComponent,
    InstallPromptComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatProgressBarModule,
    TranslateModule,
    SheetDismissDirective,
    ShowPickerOnClickDirective,
  ],
  exports: [
    TranslateModule,
    CommonModule,
    ReactiveFormsModule,
    CalculateRemainingDaysPipe,
    EntityImagePipe,
    TruncatePipe,
    ParseDateTagPipe,
    TranslateInPipe,
    ControlsMenuComponent,
    RandomColorDirective,
    DatePipe,
    PasswordVisibilityToggleComponent,
    InstallPromptComponent,
    SheetDismissDirective,
    ShowPickerOnClickDirective,
  ],
  providers: [
    EntityImagePipe,
    CalculateRemainingDaysPipe,
    DatePipe,
    TruncatePipe,
    ParseDateTagPipe,
    TranslatePipe,
    TranslateInPipe,
    TOAST_DEFAULT_OPTIONS,
  ],
})
export class SharedModule {}
