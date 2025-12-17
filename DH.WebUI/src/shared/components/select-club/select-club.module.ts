import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SelectClubComponent } from './select-club.component';

@NgModule({
  declarations: [SelectClubComponent],
  exports: [SelectClubComponent],
  imports: [BrowserModule],
})
export class SelectClubModule {}
