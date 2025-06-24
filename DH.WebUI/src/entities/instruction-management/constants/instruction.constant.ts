import { Instruction, LinkInfoType } from '../models/instruction.model';

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
        linkInfo: [
          {
            header: 'Find the Game that you want to reserve',
            content: [
              {
                description:
                  'Start by selecting the game library at the bottom left icon of the screen. This will take you to the game library where you can browse available games.',
                imagePath:
                  '/shared/assets/images/icons/menu_book-icon-blue.svg',
              },
              {
                description:
                  'Select the game you want to reserve from the list of available games. You can use the search bar to quickly find a specific game.',
                imagePath:
                  '/shared/assets/images/instructions/Reservations/Game/game_reservation.gif',
              },
              {
                description:
                  'Select after what time you will arrive and how many players will join to you. We will reserve a table for you.',
                imagePath:
                  '/shared/assets/images/instructions/Reservations/Game/game_reservation.gif',
              },
              {
                description:
                  'You will receive a confirmation/notification message once your reservation is successfully approved/declined.',
                imagePath:
                  '/shared/assets/images/instructions/Reservations/Game/game_reservation.gif',
              },
            ],
          },
           {
            header: 'Find the Game that you want to reserve',
            content: [
              {
                description:
                  'Start by selecting the game library at the bottom left icon of the screen. This will take you to the game library where you can browse available games.',
                imagePath:
                  '/shared/assets/images/icons/menu_book-icon-blue.svg',
              }
            ],
          },
          // {
          //   text: 'Create Your Reservation',
          //   infoType: LinkInfoType.Header,
          // },
          // {
          //   text: 'Start by selecting the game you want to play and specifying the number of participants joining you.',
          //   infoType: LinkInfoType.Text,
          // },
          // {
          //   text: 'Wait for Approval',
          //   infoType: LinkInfoType.Header,
          // },
          // {
          //   text: `Once you've submitted your reservation, you will need to wait for the club to approve or decline it. The response time may vary depending on the club's workload, so please allow a few minutes for processing.`,
          //   infoType: LinkInfoType.Text,
          // },
          // { text: 'Reservation Confirmation', infoType: LinkInfoType.Header },
          // {
          //   text: 'If there is available space for your group, your reservation will be approved.',
          //   infoType: LinkInfoType.Text,
          // },
          // {
          //   text: `QR Code & Check-in`,
          //   infoType: LinkInfoType.Header,
          // },
          // {
          //   text: `After approval, a QR code will be generated for your reservation. You must present this QR code to the club staff, who will scan it to verify its validity.`,
          //   infoType: LinkInfoType.Text,
          // },
          // {
          //   text: `shared/.....`,
          //   infoType: LinkInfoType.Image,
          // },
          // {
          //   text: 'Additional Information',
          //   infoType: LinkInfoType.Header,
          // },
          // {
          //   text: `Under the "Information" tab, you will find any messages from the staff, including important details or special instructions regarding your reservation.`,
          //   infoType: LinkInfoType.Text,
          // },
          // {
          //   text: ``,
          //   infoType: LinkInfoType.Empty,
          // },
          // {
          //   text: `For instructions on how to make a table reservation, check the guide here: `,
          //   infoType: LinkInfoType.Text,
          // },
          // {
          //   text: ``,
          //   infoType: LinkInfoType.Link,
          // },
        ],
      },
      {
        name: 'Table',
        path: '/instructions/reservation/table',
        image: '/shared...',
        description: 'Reserve a table',
        linkInfo: [],
      },
      {
        name: 'Combination',
        path: '/instructions/reservation/combination',
        image: '/shared...',
        description: 'Reserve a combination of both',
        linkInfo: [],
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
        linkInfo: [],
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
        linkInfo: [],
      },
      {
        name: 'Rewards History',
        path: '/instructions/challenges/rewards',
        image: '/shared...',
        description: 'Check your rewards history',
        linkInfo: [],
      },
      {
        name: 'Leaderboard',
        path: '/instructions/challenges/leaderboard',
        image: '/shared...',
        description: 'Check the leaderboard',
        linkInfo: [],
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
        linkInfo: [],
      },
      {
        name: 'Find Players',
        path: '/instructions/meeples/find',
        image: '/shared...',
        description: 'Find players',
        linkInfo: [],
      },
    ],
  },
};
