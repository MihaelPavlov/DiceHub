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
        description: `Learn how to reserve a game step by step.`,
        linkInfo: [
          {
            header: 'How to Reserve a Game',
            description: `Tap the game library icon in the bottom-left corner. Use the list or search bar to find a game you want to play.
            After selecting a game, click on Availability 
            Select your arrival time and number of players — we’ll reserve a table for you. 
            You’ll be notified when your reservation is approved or declined. 
            If approved, please arrive 2–10 minutes before the timer ends.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/game_reservation_part_1.gif',
          },
          {
            header: 'Reservation Approved: What’s Next',
            description: `Once your reservation is approved, you'll see a QR code and an info panel.
              You can access this panel anytime from the red Games Tab in the top-right corner (it will glow when updated).
              This panel shows important details and updates about your reservation.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/game_reservation_part_2.gif',
          },
          {
            header: 'Messages from Staff',
            description: `Your info panel may include notes from staff about your reservation — make sure to read them.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/7.png',
          },
          {
            header: 'Checking In with Your QR Code',
            description: `Show your QR code to the club staff when you arrive. They’ll scan it to confirm your reservation.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/9.png',
          },
          {
            header: 'If Your Reservation is Declined',
            description: `If your reservation is not approved, you’ll see a message explaining why it was declined.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/11.png',
          },
          {
            header: 'If No One Responds to Your Reservation',
            description: `If your reservation isn’t acknowledged in time, you’ll get a additional message.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Game/13.png',
          },
        ],
      },
      {
        name: 'Table',
        path: '/instructions/reservation/table',
        image: '/shared...',
        description: 'Learn how to reserve a table.',
        linkInfo: [
          {
            header: 'How to Reserve a Table',
            description: `Tap the circular icon with crossed swords at the bottom center of the screen. 
            Then, tap the top icon to open the Club Space Management page. 
            There, you can book a table, choosing your arrival time, and entering the number of players. 
            You’ll receive a notification once your reservation is approved or declined.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Table/table_reservation_part_1.gif',
          },
          {
            header: 'Reservation Approved: What’s Next',
            description: `If your reservation is approved, a QR code and all reservation details will appear in your info panel under Club Space Management page.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Table/1.png',
          },
          {
            header: 'Message from Staff',
            description: `Sometimes, staff may leave you a note in the info panel with important information about your reservation.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/Table/3.png',
          },
        ],
      },
      {
        name: 'Combination',
        path: '/instructions/reservation/combination',
        image: '/shared...',
        description: 'Reserve a combination of both',
        linkInfo: [
          {
            header: 'Manage your reservation room',
            description: `Start by clicking on the blue circle at the center of the bottom center of the screen. And then click on the first icon at the top.
               From there you will be navigated to the Club Space Management page, where you can manage your reservation room.
               You can update your reservation and remove players`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/find_my_reservation.gif',
          },
        ],
      },
      {
        name: 'Tip #1 Manage Your Room',
        path: '/instructions/reservation/manage-room',
        image: '/shared...',
        description:
          'Easily manage your reservation room — see who’s in, update players, and adjust your settings.',
        linkInfo: [
          {
            header: 'How to Manage Your Room',
            description: `Tap the blue circle at the bottom center of the screen. Then tap the first icon at the top.
              This will take you to the Club Space Management page, where you can view and manage your reservation room.
              From there, you can update your reservation details or remove players if needed.`,
            imagePath:
              '/shared/assets/images/instructions/Reservations/find_my_reservation.gif',
          },
        ],
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
