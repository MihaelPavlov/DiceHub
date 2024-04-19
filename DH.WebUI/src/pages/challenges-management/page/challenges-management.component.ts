import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-challenges-management',
  templateUrl: 'challenges-management.component.html',
  styleUrl: 'challenges-management.component.scss',
})
export class ChallengesManagementComponent {
  constructor(private readonly router: Router) {}
}
