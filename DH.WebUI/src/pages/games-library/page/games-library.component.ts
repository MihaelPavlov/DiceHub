import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth-service';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent {
  constructor(private readonly router: Router, readonly authService: AuthService) {}

  public navigateToGameDetails(): void {
    this.router.navigateByUrl('games/1/details');
  }

  login(){
    this.authService.login({email:"rap4obg@abv.bg", password: "123456789Mm!"});
  }
  game(){
    this.authService.game({name:"test123"});
  }
  register(){
    this.authService.register({username: "rap4obg",email:"rap4obg@abv.bg", password: "123456789Mm!"});
  }
  userInfo(){
    this.authService.userinfo();

  }
  logout(){
    this.authService.logout();

  }
}
