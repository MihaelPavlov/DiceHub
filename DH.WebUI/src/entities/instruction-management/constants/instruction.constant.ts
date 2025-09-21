import { InstructionSection, LinkInfoType } from '../models/instruction.model';

export const INSTRUCTION_LINK_MAPPINGS: { [key: string]: InstructionSection } =
  {
    reservation: {
      title: 'instruction.reservation.title',
      summary: 'instruction.reservation.summary',
      imageUrl: '/shared/assets/images/icons/menu_book-icon-blue.svg',
      topics: [
        {
          title: 'instruction.reservation.topic.game.title',
          route: '/instructions/reservation/game',
          thumbnailUrl: 'instruction.reservation.topic.game.thumbnail_url',
          description: `instruction.reservation.topic.game.description`,
          steps: [
            {
              header:
                'instruction.reservation.topic.game.steps.how_to_reserve_game.header',
              description: `instruction.reservation.topic.game.steps.how_to_reserve_game.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.how_to_reserve_game.media_url',
            },
            {
              header:
                'instruction.reservation.topic.game.steps.reservation_approved.header',
              description: `instruction.reservation.topic.game.steps.reservation_approved.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.reservation_approved.media_url',
            },
            {
              header:
                'instruction.reservation.topic.game.steps.message_from_staff.header',
              description: `instruction.reservation.topic.game.steps.message_from_staff.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.message_from_staff.media_url',
            },
            {
              header:
                'instruction.reservation.topic.game.steps.checking_in_with_qr_code.header',
              description: `instruction.reservation.topic.game.steps.checking_in_with_qr_code.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.checking_in_with_qr_code.media_url',
            },
            {
              header:
                'instruction.reservation.topic.game.steps.reservation_is_declined.header',
              description: `instruction.reservation.topic.game.steps.reservation_is_declined.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.reservation_is_declined.media_url',
            },
            {
              header:
                'instruction.reservation.topic.game.steps.no_one_responds_to_reservation.header',
              description: `instruction.reservation.topic.game.steps.no_one_responds_to_reservation.description`,
              mediaUrl:
                'instruction.reservation.topic.game.steps.no_one_responds_to_reservation.media_url',
            },
          ],
        },
        {
          title: 'instruction.reservation.topic.table.title',
          route: '/instructions/reservation/table',
          thumbnailUrl: 'instruction.reservation.topic.table.thumbnail_url',
          description: 'instruction.reservation.topic.table.description',
          steps: [
            {
              header:
                'instruction.reservation.topic.table.steps.how_to_reserve_table.header',
              description: `instruction.reservation.topic.table.steps.how_to_reserve_table.description`,
              mediaUrl:
                'instruction.reservation.topic.table.steps.how_to_reserve_table.media_url',
            },
            {
              header:
                'instruction.reservation.topic.table.steps.reservation_approved.header',
              description: `instruction.reservation.topic.table.steps.reservation_approved.description`,
              mediaUrl:
                'instruction.reservation.topic.table.steps.reservation_approved.media_url',
            },
            {
              header:
                'instruction.reservation.topic.table.steps.message_from_staff.header',
              description: `instruction.reservation.topic.table.steps.message_from_staff.description`,
              mediaUrl:
                'instruction.reservation.topic.table.steps.message_from_staff.media_url',
            },
            {
              header:
                'instruction.reservation.topic.table.steps.reservation_declined.header',
              description: `instruction.reservation.topic.table.steps.reservation_declined.description`,
              mediaUrl:
                'instruction.reservation.topic.table.steps.reservation_declined.media_url',
            },
          ],
        },
        {
          title: 'instruction.reservation.topic.combination.title',
          route: '/instructions/reservation/combination',
          thumbnailUrl:
            'instruction.reservation.topic.combination.thumbnail_url',
          description: 'instruction.reservation.topic.combination.description',
          steps: [
            {
              header:
                'instruction.reservation.topic.combination.steps.reserve_table.header',
              description: `instruction.reservation.topic.combination.steps.reserve_table.description`,
              action: {
                label:
                  'instruction.reservation.topic.combination.steps.reserve_table.action_label',
                url: '/instructions/reservation/table',
              },
              mediaUrl:
                'instruction.reservation.topic.combination.steps.reserve_table.media_url',
            },
            {
              header:
                'instruction.reservation.topic.combination.steps.game_reservation.header',
              description: `instruction.reservation.topic.combination.steps.game_reservation.description`,
              mediaUrl:
                'instruction.reservation.topic.combination.steps.game_reservation.media_url',
            },
            {
              header:
                'instruction.reservation.topic.combination.steps.game_reservation_approved.header',
              description: `instruction.reservation.topic.combination.steps.game_reservation_approved.description`,
              mediaUrl:
                'instruction.reservation.topic.combination.steps.game_reservation_approved.media_url',
            },
            {
              header:
                'instruction.reservation.topic.combination.steps.game_reservation_declined.header',
              description: `instruction.reservation.topic.combination.steps.game_reservation_declined.description`,
              mediaUrl:
                'instruction.reservation.topic.combination.steps.game_reservation_declined.media_url',
            },
          ],
        },
        {
          title: 'instruction.reservation.topic.manage_room.title',
          route: '/instructions/reservation/manage-room',
          thumbnailUrl:
            'instruction.reservation.topic.manage_room.thumbnail_url',
          description: 'instruction.reservation.topic.manage_room.description',
          steps: [
            {
              header:
                'instruction.reservation.topic.manage_room.steps.manage_room.header',
              description:
                'instruction.reservation.topic.manage_room.steps.manage_room.description',
              mediaUrl:
                'instruction.reservation.topic.manage_room.steps.manage_room.media_url',
            },
          ],
        },
      ],
    },
    events: {
      title: 'instruction.events.title',
      summary: 'instruction.events.summary',
      imageUrl: '/shared/assets/images/icons/stadium-icon-blue.svg',
      topics: [
        {
          title: 'instruction.events.topic.join_event.title',
          route: '/instructions/events/all',
          thumbnailUrl: 'instruction.events.topic.join_event.thumbnail_url',
          description: 'instruction.events.topic.join_event.description',
          steps: [
            {
              header:
                'instruction.events.topic.join_event.steps.join_event.header',
              description:
                'instruction.events.topic.join_event.steps.join_event.description',
              mediaUrl:
                'instruction.events.topic.join_event.steps.join_event.media_url',
            },
          ],
        },
        {
          title: 'instruction.events.topic.leave_event.title',
          route: '/instructions/events/all',
          thumbnailUrl: 'instruction.events.topic.leave_event.thumbnail_url',
          description: 'instruction.events.topic.leave_event.description',
          steps: [
            {
              header:
                'instruction.events.topic.leave_event.steps.leave_event.header',
              description:
                'instruction.events.topic.leave_event.steps.leave_event.description',
              mediaUrl:
                'instruction.events.topic.leave_event.steps.leave_event.media_url',
            },
          ],
        },
        {
          title: 'instruction.events.topic.notifications.title',
          route: '/instructions/events/all',
          thumbnailUrl: 'instruction.events.topic.notifications.thumbnail_url',
          description: 'instruction.events.topic.notifications.description',
          steps: [
            {
              header:
                'instruction.events.topic.notifications.steps.notifications.header',
              description:
                'instruction.events.topic.notifications.steps.notifications.description',
              mediaUrl:
                'instruction.events.topic.notifications.steps.notifications.media_url',
            },
          ],
        },
      ],
    },
    challenges: {
      title: 'instruction.challenges.title',
      summary: `instruction.challenges.summary`,
      imageUrl: '/shared/assets/images/icons/swords_icon-blue.svg',
      topics: [
        {
          title: 'instruction.challenges.topic.info.title',
          route: '/instructions/challenges/info',
          thumbnailUrl: 'instruction.challenges.topic.info.thumbnail_url',
          description: 'instruction.challenges.topic.info.description',
          steps: [
            {
              header:
                'instruction.challenges.topic.info.steps.where_to_find.header',
              description:
                'instruction.challenges.topic.info.steps.where_to_find.description',
              mediaUrl:
                'instruction.challenges.topic.info.steps.where_to_find.media_url',
            },
            {
              header:
                'instruction.challenges.topic.info.steps.how_to_complete.header',
              description:
                'instruction.challenges.topic.info.steps.how_to_complete.description',
              mediaUrl:
                'instruction.challenges.topic.info.steps.how_to_complete.media_url',
            },
            {
              header:
                'instruction.challenges.topic.info.steps.when_new_challenges.header',
              description:
                'instruction.challenges.topic.info.steps.when_new_challenges.description',
              mediaUrl:
                'instruction.challenges.topic.info.steps.when_new_challenges.media_url',
            },
          ],
        },
        {
          title: 'instruction.challenges.topic.rewards.title',
          route: '/instructions/challenges/rewards',
          thumbnailUrl: 'instruction.challenges.topic.rewards.thumbnail_url',
          description: 'instruction.challenges.topic.rewards.description',
          steps: [
            {
              header:
                'instruction.challenges.topic.rewards.steps.where_to_see.header',
              description:
                'instruction.challenges.topic.rewards.steps.where_to_see.description',
              mediaUrl:
                'instruction.challenges.topic.rewards.steps.where_to_see.media_url',
            },
            {
              header:
                'instruction.challenges.topic.rewards.steps.how_to_claim.header',
              description:
                'instruction.challenges.topic.rewards.steps.how_to_claim.description',
              mediaUrl:
                'instruction.challenges.topic.rewards.steps.how_to_claim.media_url',
            },
          ],
        },
        {
          title: 'instruction.challenges.topic.leaderboard.title',
          route: '/instructions/challenges/leaderboard',
          thumbnailUrl:
            'instruction.challenges.topic.leaderboard.thumbnail_url',
          description: 'instruction.challenges.topic.leaderboard.description',
          steps: [
            {
              header:
                'instruction.challenges.topic.leaderboard.steps.where_to_view.header',
              description:
                'instruction.challenges.topic.leaderboard.steps.where_to_view.description',
              mediaUrl:
                'instruction.challenges.topic.leaderboard.steps.where_to_view.media_url',
            },
          ],
        },
      ],
    },
    meeples: {
      title: 'instruction.meeples.title',
      summary: `instruction.meeples.summary`,
      imageUrl: '/shared/assets/images/icons/group-icon-blue.svg',
      topics: [
        {
          title: 'instruction.meeples.topic.groups.title',
          route: '/instructions/meeples/groups',
          thumbnailUrl: 'instruction.meeples.topic.groups.thumbnail_url',
          description: 'instruction.meeples.topic.groups.description',
          steps: [
            {
              header:
                'instruction.meeples.topic.groups.steps.view_available.header',
              description:
                'instruction.meeples.topic.groups.steps.view_available.description',
              mediaUrl:
                'instruction.meeples.topic.groups.steps.view_available.media_url',
            },
          ],
        },
        {
          title: 'instruction.meeples.topic.create_room.title',
          route: '/instructions/meeples/find',
          thumbnailUrl: 'instruction.meeples.topic.create_room.thumbnail_url',
          description: 'instruction.meeples.topic.create_room.description',
          steps: [
            {
              header:
                'instruction.meeples.topic.create_room.steps.create.header',
              description:
                'instruction.meeples.topic.create_room.steps.create.description',
              mediaUrl:
                'instruction.meeples.topic.create_room.steps.create.media_url',
            },
          ],
        },
        {
          title: 'instruction.meeples.topic.join_room.title',
          route: '/instructions/meeples/find',
          thumbnailUrl: 'instruction.meeples.topic.join_room.thumbnail_url',
          description: 'instruction.meeples.topic.join_room.description',
          steps: [
            {
              header:
                'instruction.meeples.topic.join_room.steps.join_existing.header',
              description:
                'instruction.meeples.topic.join_room.steps.join_existing.description',
              mediaUrl:
                'instruction.meeples.topic.join_room.steps.join_existing.media_url',
            },
            {
              header:
                'instruction.meeples.topic.join_room.steps.room_info_chat.header',
              description:
                'instruction.meeples.topic.join_room.steps.room_info_chat.description',
              mediaUrl:
                'instruction.meeples.topic.join_room.steps.room_info_chat.media_url',
            },
          ],
        },
      ],
    },
  };
