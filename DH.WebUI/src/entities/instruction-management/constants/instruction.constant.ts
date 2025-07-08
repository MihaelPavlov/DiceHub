import { InstructionSection, LinkInfoType } from '../models/instruction.model';

export const INSTRUCTION_LINK_MAPPINGS: { [key: string]: InstructionSection } =
  {
    reservation: {
      title: 'Reservations',
      summary: 'Reserve a game, table, or a combination of both',
      imageUrl: '/shared/assets/images/icons/menu_book-icon-blue.svg',
      topics: [
        {
          title: 'Game',
          route: '/instructions/reservation/game',
          thumbnailUrl: '/shared/assets/images/instructions/Reservations/Game/3.png',
          description: `Learn how to reserve a game step by step.`,
          steps: [
            {
              header: 'How to Reserve a Game',
              description: `Tap the game library icon in the bottom-left corner. Use the list or search bar to find a game you want to play.
            After selecting a game, click on Availability 
            Select your arrival time and number of players — we’ll reserve a table for you. 
            You’ll be notified when your reservation is approved or declined. 
            If approved, please arrive 2–10 minutes before the timer ends.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/game_reservation_part_1.gif',
            },
            {
              header: 'Reservation Approved: What’s Next',
              description: `Once your reservation is approved, you'll see a QR code and an info panel.
              You can access this panel anytime from the red Games Tab in the top-right corner (it will glow when updated).
              This panel shows important details and updates about your reservation.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/game_reservation_part_2.gif',
            },
            {
              header: 'Messages from Staff',
              description: `Your info panel may include notes from staff about your reservation — make sure to read them.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/7.png',
            },
            {
              header: 'Checking In with Your QR Code',
              description: `Show your QR code to the club staff when you arrive. They’ll scan it to confirm your reservation.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/9.png',
            },
            {
              header: 'If Your Reservation is Declined',
              description: `If your reservation is not approved, you’ll see a message explaining why it was declined.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/11.png',
            },
            {
              header: 'If No One Responds to Your Reservation',
              description: `If your reservation isn’t acknowledged in time, you’ll get a additional message.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Game/13.png',
            },
          ],
        },
        {
          title: 'Table',
          route: '/instructions/reservation/table',
          thumbnailUrl: '/shared/assets/images/instructions/Reservations/Table/1.png',
          description: 'Learn how to reserve a table.',
          steps: [
            {
              header: 'How to Reserve a Table',
              description: `Tap the circular icon with crossed swords at the bottom center of the screen. 
            Then, tap the top icon to open the Club Space Management page. 
            There, you can book a table, choosing your arrival time, and entering the number of players. 
            You’ll receive a notification once your reservation is approved or declined.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Table/table_reservation_part_1.gif',
            },
            {
              header: 'Reservation Approved: What’s Next',
              description: `If your reservation is approved, a QR code and all reservation details will appear in your info panel under Club Space Management page.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Table/1.png',
            },
            {
              header: 'Message from Staff',
              description: `Sometimes, staff may leave you a note in the info panel with important information about your reservation.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Table/3.png',
            },
            {
              header: 'If Your Reservation is Declined',
              description: `If your reservation is not approved, try selecting a different time or day.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Table/4.png',
            },
          ],
        },
        {
          title: 'Combination',
          route: '/instructions/reservation/combination',
          thumbnailUrl: '/shared/assets/images/instructions/Reservations/Combination/2.png',
          description:
            'Learn how to combine both a table and a game reservation into one seamless experience.',
          steps: [
            {
              header: 'Step 1: Reserve Your Table',
              description: `Start by learning how to book a table. Follow the instructions provided in the link below to secure your spot.`,
              action: {
                label: 'How to Reserve a Table',
                url: '/instructions/reservation/table',
              },
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Table/table_reservation_part_1.gif',
            },
            {
              header: 'Step 2: Game Reservation on the Day of Your Visit',
              description: `On the day of your table reservation, you’ll receive a special message on the game availability page. This message will include your table details.   
            
            Exactly **1 hour before** your reserved time, you’ll be able to select and reserve a game.

            Once you request a game reservation, it will either be approved or declined depending on availability.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Combination/2.png',
            },
            {
              header: 'If Your Game Reservation is Approved',
              description: `Great! Both your table and your selected game will be ready and waiting for you at the venue.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Combination/3.png',
            },
            {
              header: 'If Your Game Reservation is Declined',
              description: `Don’t worry—your table reservation is still confirmed. You can either:

            - Try reserving a different game, or
            - Arrive and choose from the games available on-site.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/Combination/4.png',
            },
          ],
        },
        {
          title: 'Tip #1 Manage Your Room',
          route: '/instructions/reservation/manage-room',
          thumbnailUrl: '/shared/assets/images/instructions/Reservations/find_my_reservation.gif',
          description:
            'Easily manage your reservation room — see who’s in, update players, and adjust your settings.',
          steps: [
            {
              header: 'How to Manage Your Room',
              description: `Tap the blue circle at the bottom center of the screen. Then tap the first icon at the top.
              This will take you to the Club Space Management page, where you can view and manage your reservation room.
              From there, you can update your reservation details or remove players if needed.`,
              mediaUrl:
                '/shared/assets/images/instructions/Reservations/find_my_reservation.gif',
            },
          ],
        },
      ],
    },
    events: {
      title: 'Events',
      summary:
        'Join exciting events and connect with the community through games, challenges, and fun activities.',
      imageUrl: '/shared/assets/images/icons/stadium-icon-blue.svg',
      topics: [
        {
          title: 'How to Join an Event',
          route: '/instructions/events/all',
          thumbnailUrl: '/shared/assets/images/instructions/Events/join_event.gif',
          description:
            'Find and join upcoming community events directly from your app.',
          steps: [
            {
              header: 'Step-by-Step: Joining an Event',
              description: `1. Tap the **Events** tab at the bottom of your screen.  
            2. Browse through the list of upcoming events.  
            3. Select an event to view full details like time, date, and description.  
            4. Tap **Join Event** to secure your spot!`,
              mediaUrl:
                '/shared/assets/images/instructions/Events/join_event.gif',
            },
          ],
        },
        {
          title: 'How to Leave an Event',
          route: '/instructions/events/all',
          thumbnailUrl: '/shared/assets/images/instructions/Events/leave_event.gif',
          description: 'Can’t make it? Easily leave an event you’ve joined.',
          steps: [
            {
              header: 'Removing Yourself from an Event',
              description: `1. Open the **Events** tab.  
              2. Go to the **My Events** section.  
              3. Tap the event you want to leave.  
              4. Tap **Leave Event** at the bottom of the screen.`,
              mediaUrl:
                '/shared/assets/images/instructions/Events/leave_event.gif',
            },
          ],
        },
        {
          title: 'Stay Informed with Notifications',
          route: '/instructions/events/all',
          thumbnailUrl: '/shared/assets/images/instructions/Events/2.png',
          description: 'Get notified when new events are created.',
          steps: [
            {
              header: 'How Event Notifications Work',
              description: `When your club creates a new event, you'll receive a notification.  
              Tap the notification to view event details and decide whether to join.`,
              mediaUrl: '/shared/assets/images/instructions/Events/2.png',
            },
          ],
        },
      ],
    },
    challenges: {
      title: 'Challenge & Rewards',
      summary: `Take part in exciting challenges and earn awesome rewards! 
        This is your main hub for everything related to challenges, rewards, and friendly competition.`,
      imageUrl: '/shared/assets/images/icons/swords_icon-blue.svg',
      topics: [
        {
          title: 'Challenges',
          route: '/instructions/challenges/info',
          thumbnailUrl: '/shared/assets/images/instructions/Challenges/1.png',
          description: 'Learn where, when, and how to complete challenges.',
          steps: [
            {
              header: 'Where to Find Challenges',
              description: `Tap the blue circle at the bottom center of the screen, then tap the middle icon.

              This will open your Challenge Page, where you can view both your current challenges and earned rewards.

              The first tab (top left) will show whether you're participating in a weekly or monthly challenge period.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/1.png',
            },
            {
              header: 'How to Complete a Challenge',
              description: `Each challenge tells you which game to play, how many times to play it, and how many points you’ll earn.
              
              These points are added to your total period score.
              
              To successfully complete a challenge, you must start the required game and play it for at least the average game time.
              
              For example: if the average play time for Ghost Stories is 55 minutes, you must play it for at least 55 minutes to get the reward.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/1.png',
            },
            {
              header: 'When Do I Get New Challenges?',
              description: `A new challenge is usually generated a few hours after you complete the previous one.
              Occasionally, you’ll receive special or time-limited challenges too.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/1.png',
            },
          ],
        },
        {
          title: 'Rewards',
          route: '/instructions/challenges/rewards',
          thumbnailUrl: '/shared/assets/images/instructions/Challenges/2.png',
          description: 'Find out how to earn and redeem your rewards.',
          steps: [
            {
              header: 'Where to See Your Rewards',
              description: `Tap the blue circle at the bottom center, then the middle icon to open the Challenge Page.
              Switch to the second tab (top left) to view all the rewards you’ve earned.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/2.png',
            },
            {
              header: 'How to Claim Your Reward',
              description: `Once you earn a reward, you’ll have a limited number of days to claim it before it expires.

              To redeem it, visit the club and press the “Use” button inside the app.

              Show the generated QR code to a staff member to receive your reward.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/3.png',
            },
          ],
        },
        {
          title: 'Leaderboard',
          route: '/instructions/challenges/leaderboard',
          thumbnailUrl: '/shared/assets/images/instructions/Challenges/4.png',
          description: 'Check the leaderboard',
          steps: [
            {
              header: 'Where to View the Leaderboard?',
              description: `Go to your Profile page, then find and tap the Challenge Leaderboard section.

              You can view your ranking along with the scores of other players.`,
              mediaUrl: '/shared/assets/images/instructions/Challenges/4.png',
            },
          ],
        },
      ],
    },
    meeples: {
      title: 'Meeples',
      summary:
        `Looking for people to play with? 
        The Meeples section helps you find or create a group of players who want to enjoy the same game as you. Meet new people, connect, and have fun together!`,
      imageUrl: '/shared/assets/images/icons/group-icon-blue.svg',
      topics: [
        {
          title: 'Meeple Groups / Find Players',
          route: '/instructions/meeples/groups',
          thumbnailUrl: '/shared/assets/images/instructions/Meeples/2.png',
          description:
            'Browse and join available groups where players are looking for others to play games with.',
          steps: [
            {
              header: 'View Available Groups & Rooms',
              description: `Head to your Meeple page to see a list of open rooms created by other players.
You can browse these and join any group that matches your interest.`,
              mediaUrl: '/shared/assets/images/instructions/Meeples/2.png',
            },
          ],
        },
        {
          title: 'Create a Room',
          route: '/instructions/meeples/find',
          thumbnailUrl: '/shared/assets/images/instructions/Meeples/1.png',
          description: 'Set up your own room and wait for others to join you for a game session.',
          steps: [
            {
              header: 'Create a Meeple Room',
              description: `On the Meeple page, tap the green plus (+) button at the top right.
You’ll be taken to the room creation page, where you can set the game, time, and number of players.`,
              mediaUrl: '/shared/assets/images/instructions/Meeples/1.png',
            },
          ],
        },
        {
          title: 'Join Room',
          route: '/instructions/meeples/find',
          thumbnailUrl: '/shared/assets/images/instructions/Meeples/3.png',
          description: 'Join room and meet the new people',
          steps: [
            {
              header: 'Join an Existing Room',
              description: `Browse the list of rooms on the Meeple page. 
              Tap on a room you’re interested in to view more details and join it.`,
              mediaUrl: '/shared/assets/images/instructions/Meeples/3.png',
            },
            {
              header: 'Room Info & Chat',
              description: `Inside each room, you’ll find details about the selected game, scheduled time, and players.
You can also open the chat to talk to other participants, coordinate, or get updates.`,
              mediaUrl: '/shared/assets/images/instructions/Meeples/4.png', 
            },
          ],
        },
      ],
    },
  };
