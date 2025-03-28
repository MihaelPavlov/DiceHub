import { Instruction } from '../models/instruction.model';

export const INSTRUCTION_LINK_MAPPINGS: { [key: string]: Instruction } = {
  reservation: {
    header: 'Reservations',
    description: 'Reserve a game, table, or a combination of both',
    imagePath: '/shared/assets/images/icons/menu_book-icon-blue.svg',
    links: [
      {
        name: 'Game',
        path: '/instructions/reservation/game',
        image: '/shared...',
        description: 'Reserve a game',
      },
      {
        name: 'Table',
        path: '/instructions/reservation/table',
        image: '/shared...',
        description: 'Reserve a table',
      },
      {
        name: 'Combination',
        path: '/instructions/reservation/combination',
        image: '/shared...',
        description: 'Reserve a combination of both',
      },
    ],
  },
  events: {
    header: 'Events',
    description: 'Events, to join our fun, exciting activities as a community',
    imagePath: '/shared/assets/images/icons/stadium-icon-blue.svg',
    links: [
      {
        name: 'Join Event',
        path: '/instructions/events/all',
        image: '/shared...',
        description: 'Join our events',
      },
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
        image: '/shared...',
        description: 'Join our ongoing challenges',
      },
      {
        name: 'Rewards History',
        path: '/instructions/challenges/rewards',
        image: '/shared...',
        description: 'Check your rewards history',
      },
      {
        name: 'Leaderboard',
        path: '/instructions/challenges/leaderboard',
        image: '/shared...',
        description: 'Check the leaderboard',
      },
    ],
  },
  meeples: {
    header: 'Meeples',
    description:
      'The place where you can join to a group of player, who want to enjoy the game with you.',
    imagePath: '/shared/assets/images/icons/group-icon-blue.svg',
    links: [
      {
        name: 'Meeple Groups',
        path: '/instructions/meeples/groups',
        image: '/shared...',
        description: 'Join a group',
      },
      {
        name: 'Find Players',
        path: '/instructions/meeples/find',
        image: '/shared...',
        description: 'Find players',
      },
    ],
  },
};
