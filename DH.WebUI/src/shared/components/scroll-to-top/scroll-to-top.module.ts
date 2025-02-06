import { NgModule } from '@angular/core';
import { ScrollTopComponent } from './scroll-to-top.component';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [ScrollTopComponent],
  exports: [ScrollTopComponent],
  imports: [BrowserModule],
})
export class ScrollToTopModule {}
