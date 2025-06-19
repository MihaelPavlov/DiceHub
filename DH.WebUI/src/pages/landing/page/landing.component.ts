import { Component } from '@angular/core';
import { Router } from '@angular/router';
import {  ROUTE } from '../../../shared/configs/route.config';

@Component({
  selector: 'app-landing',
  templateUrl: 'landing.component.html',
  styleUrl: 'landing.component.scss',
})
export class LandingComponent {
  constructor(private readonly router: Router) {}

  public onLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public onRegister(): void {
    this.router.navigateByUrl(ROUTE.REGISTER);
  }

  public onInstructions(): void {
    this.router.navigateByUrl(ROUTE.INSTRUCTIONS);
  }
}
