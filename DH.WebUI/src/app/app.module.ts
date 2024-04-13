import { NgModule } from '@angular/core';
import { AppComponent } from './app-component/app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { MenuModule } from '../widgets/menu/menu.module';
import { RouterOutlet } from '@angular/router';
import { AppRoutingModule } from './app-routes/app-routes.module';
import { HeaderModule } from '../widgets/header/header.module';

@NgModule({
  declarations: [AppComponent],
  exports: [BrowserModule],
  providers: [],
  bootstrap: [AppComponent],
  imports: [HttpClientModule,AppRoutingModule, BrowserModule, MenuModule,HeaderModule, RouterOutlet],
})
export class AppModule {}
