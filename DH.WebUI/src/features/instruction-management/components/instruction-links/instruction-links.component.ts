import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

interface LinksDescription {
  header: string;
  description: string;
  imagePath: string;
  links: Link[];
}
interface Link {
  name: string;
  path: string;
}

@Component({
  selector: 'app-instruction-links',
  templateUrl: 'instruction-links.component.html',
  styleUrl: 'instruction-links.component.scss',
})
export class InstructionLinksComponent implements OnInit {
  public currentLink!: LinksDescription;
  public linkMappings: {
    [key: string]: LinksDescription;
  } = {
    reservation: {
      header: 'Reservations',
      description: 'Reserve a game, table, or a combination of both',
      imagePath: '/shared/assets/images/icons/menu_book-icon-blue.svg',
      links: [
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
    },
    events: {
      header: 'Events',
      description:
        'Events, to join our fun, exciting activities as a community',
      imagePath: '/shared/assets/images/icons/stadium-icon-blue.svg',
      links: [
        { name: 'All Events', path: '/instructions/events/all' },
        { name: 'Your Bookings', path: '/instructions/events/bookings' },
        { name: 'Past Events', path: '/instructions/events/past' },
      ],
    },
    challenges: {
      header: 'Challenge & Rewards',
      description:
        'Join our challenges and earn rewards. These place is your go-to for all things challenges and rewards.',
      imagePath: '/shared/assets/images/icons/swords_icon-blue.svg',
      links: [
        {
          name: 'Ongoing Challenges',
          path: '/instructions/challenges/ongoing',
        },
        {
          name: 'Rewards History',
          path: '/instructions/challenges/rewards',
        },
        {
          name: 'Leaderboard',
          path: '/instructions/challenges/leaderboard',
        },
      ],
    },
    meeples: {
      header: 'Meeples',
      description:
        'The place where you can join to a group of player, who want to enjoy the game with you.',
      imagePath: '/shared/assets/images/icons/group-icon-blue.svg',
      links: [
        { name: 'Meeple Groups', path: '/instructions/meeples/groups' },
        { name: 'Find Players', path: '/instructions/meeples/find' },
      ],
    },
  };

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    // Listen for changes in the current route and update the links dynamically
    this.activatedRoute.url.subscribe((urlSegments) => {
      const currentPath = urlSegments[0]?.path; // Extract the first path segment

      if (currentPath && this.linkMappings[currentPath]) {
        this.currentLink = this.linkMappings[currentPath];
      }
    });
  }
}
