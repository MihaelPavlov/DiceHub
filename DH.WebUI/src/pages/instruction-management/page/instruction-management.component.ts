import { Component } from '@angular/core';

@Component({
  selector: 'app-instruction-management',
  templateUrl: 'instruction-management.component.html',
  styleUrl: 'instruction-management.component.scss',
})
export class InstructionManagementComponent {
  public defaultLinks = [
    { name: 'Reservations', path: '/instructions/reservation' },
    { name: 'Events', path: '/instructions/events' },
    { name: 'Challenges & Rewards', path: '/instructions/challenges' },
    { name: 'Meeples', path: '/instructions/meeples' },
  ];
}
