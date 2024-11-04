import { NgModule } from '@angular/core';
import { AssistiveTouchComponent } from './assistive-touch.component';
import { BrowserModule } from '@angular/platform-browser';
import { NotificationsDialog } from './notifications/notifications.dialog';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';

@NgModule({
  declarations: [AssistiveTouchComponent, NotificationsDialog],
  exports: [AssistiveTouchComponent],
  imports: [BrowserModule, MatDialogActions, MatDialogClose],
})
export class AssistiveTouchModule {}
