import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent {
  constructor(private readonly router: Router){
    console.log('game lib');
    
  }
  public navigateToGameDetails():void{
    
    this.router.navigateByUrl("games/1/details");
  }
}
