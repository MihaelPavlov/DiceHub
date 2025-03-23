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
  description: string;
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
          description: 'Reserve a game',
        },
        {
          name: 'Table',
          path: '/instructions/reservation/table',
          description: 'Reserve a table',
        },
        {
          name: 'Combination',
          path: '/instructions/reservation/combination',
          description: 'Reserve a combination of both',
        },
      ],
    },
    events: {
      header: 'Events',
      description:
        'Events, to join our fun, exciting activities as a community',
      imagePath: '/shared/assets/images/icons/stadium-icon-blue.svg',
      links: [
        { name: 'Join Event', path: '/instructions/events/all' , description: 'Join our events' },
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
          description : 'Join our ongoing challenges',
        },
        {
          name: 'Rewards History',
          path: '/instructions/challenges/rewards',
          description : 'Check your rewards history',
        },
        {
          name: 'Leaderboard',
          path: '/instructions/challenges/leaderboard',
          description : 'Check the leaderboard',
        },
      ],
    },
    meeples: {
      header: 'Meeples',
      description:
        'The place where you can join to a group of player, who want to enjoy the game with you.',
      imagePath: '/shared/assets/images/icons/group-icon-blue.svg',
      links: [
        { name: 'Meeple Groups', path: '/instructions/meeples/groups', description: 'Join a group' },
        { name: 'Find Players', path: '/instructions/meeples/find', description: 'Find players' },
      ],
    },
  };

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  public ngOnInit(): void {
    // Listen for changes in the current route and update the links dynamically
    this.activatedRoute.url.subscribe((urlSegments) => {
      const currentPath = urlSegments[0]?.path; // Extract the first path segment

      if (currentPath && this.linkMappings[currentPath]) {
        this.currentLink = this.linkMappings[currentPath];
      }
    });
  }
}
