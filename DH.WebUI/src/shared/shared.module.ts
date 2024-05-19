import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AuthGuard } from '../pages/games-library/auth.guard';
import { JWT_OPTIONS, JwtHelperService, JwtModule } from '@auth0/angular-jwt';

@NgModule({
  declarations: [],
  imports: [CommonModule, JwtModule],
  exports: [CommonModule],
  providers: [
    AuthGuard,
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService,
  ],
})
export class SharedModule {}
