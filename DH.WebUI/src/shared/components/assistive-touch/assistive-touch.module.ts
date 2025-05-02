import { NgModule } from '@angular/core';
import { AssistiveTouchComponent } from './assistive-touch.component';
import { BrowserModule } from '@angular/platform-browser';
import { NotificationsDialog } from './notifications/notifications.dialog';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { FirebaseModule } from '../../firebase.module';
import { SharedModule } from "../../shared.module";

@NgModule({
  declarations: [AssistiveTouchComponent, NotificationsDialog],
  exports: [AssistiveTouchComponent],
  imports: [BrowserModule, MatDialogActions, MatDialogClose, FirebaseModule, SharedModule],
})
export class AssistiveTouchModule {}
