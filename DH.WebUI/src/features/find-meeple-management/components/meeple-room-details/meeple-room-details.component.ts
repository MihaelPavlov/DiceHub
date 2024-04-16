import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-meeple-room-details',
  templateUrl: 'meeple-room-details.component.html',
  styleUrl: 'meeple-room-details.component.scss',
})
export class MeepleRoomDetailsComponent {
  constructor(private readonly router: Router) {}
}
