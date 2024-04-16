import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-events-library',
  templateUrl: 'events-library.component.html',
  styleUrl: 'events-library.component.scss',
})
export class EventsLibraryComponent {
  constructor(private readonly router: Router) {}

  public navigateToEventDetails(): void {
    this.router.navigateByUrl("events/1/details");
  }
}
