import { NgModule } from '@angular/core';
import { AssistiveTouchComponent } from './assistive-touch.component';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [AssistiveTouchComponent],
  exports: [AssistiveTouchComponent],
  imports: [BrowserModule],
})
export class AssistiveTouchModule {}
