import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-events-library',
  templateUrl: 'events-library.component.html',
  styleUrl: 'events-library.component.scss',
})
export class EventsLibraryComponent {
  constructor(private readonly router: Router){
    console.log('events lib');
    
  }
  public navigateToGameDetails():void{
    // this.router.navigateByUrl("games/1/details");
  }
}
