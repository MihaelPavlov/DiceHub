import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-instruction-links',
  templateUrl: 'instruction-links.component.html',
  styleUrl: 'instruction-links.component.scss',
})
export class InstructionLinksComponent implements OnInit {
  public links: { name: string; path: string }[] = [];

  private linkMappings: { [key: string]: { name: string; path: string }[] } = {
    reservation: [
      {
        name: 'Game',
        path: '/instructions/reservation/game',
      },
      {
        name: 'Table',
        path: '/instructions/reservation/table',
      },
      {
        name: 'Combination',
        path: '/instructions/reservation/combination',
      },
    ],
    events: [
      { name: 'All Events', path: '/instructions/events/all' },
      { name: 'Your Bookings', path: '/instructions/events/bookings' },
      { name: 'Past Events', path: '/instructions/events/past' },
    ],
    challenges: [
      {
        name: 'Ongoing Challenges',
        path: '/instructions/challenges-rewards/ongoing',
      },
      {
        name: 'Rewards History',
        path: '/instructions/challenges-rewards/rewards',
      },
      {
        name: 'Leaderboard',
        path: '/instructions/challenges-rewards/leaderboard',
      },
    ],
    meeples: [
      { name: 'Meeple Groups', path: '/instructions/meeples/groups' },
      { name: 'Find Players', path: '/instructions/meeples/find' },
    ],
  };

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    // Listen for changes in the current route and update the links dynamically
    this.activatedRoute.url.subscribe((urlSegments) => {
      const currentPath = urlSegments[0]?.path; // Extract the first path segment

      if (currentPath && this.linkMappings[currentPath]) {
        this.links = this.linkMappings[currentPath];
      }
    });
  }
}
