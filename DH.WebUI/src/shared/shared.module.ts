import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { StringFormatPipe } from './pipe/string-format.pipe';

@NgModule({
  declarations: [StringFormatPipe],
  imports: [CommonModule, ReactiveFormsModule],
  exports: [CommonModule, ReactiveFormsModule],
  providers: [StringFormatPipe],
})
export class SharedModule {}
