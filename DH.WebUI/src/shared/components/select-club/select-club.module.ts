import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { TranslateModule } from '@ngx-translate/core';
import { SelectClubComponent } from './select-club.component';

@NgModule({
  declarations: [SelectClubComponent],
  exports: [SelectClubComponent],
  imports: [BrowserModule, TranslateModule],
})
export class SelectClubModule {}
