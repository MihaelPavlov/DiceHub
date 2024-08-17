import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { StringFormatPipe } from './pipe/string-format.pipe';
import { TOAST_DEFAULT_OPTIONS } from './models/toast.model';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ToastComponent } from './components/toast/toast.component';

@NgModule({
  declarations: [StringFormatPipe,ToastComponent],
  imports: [CommonModule, ReactiveFormsModule, MatProgressBarModule],
  exports: [CommonModule, ReactiveFormsModule],
  providers: [StringFormatPipe, TOAST_DEFAULT_OPTIONS],
})
export class SharedModule {}
