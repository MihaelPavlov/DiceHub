export const PATH = {
  GAMES: {
    CORE: 'games',
    INVENTORY: 'inventory',
    LIST: 'list',
    GET_DROPDOWN_LIST: 'get-dropdown-list',
    GET_BY_CATEGORY_ID: 'get-games-by-category',
    GET_NEW_GAMES: 'get-new-games',
    GET_RESERVED_GAMES: 'get-reserved-games',
    COPY: 'copy',
    LIKE: 'like',
    DISLIKE: 'dislike',
    RESERVATION: 'reservation',
    RESERVATION_STATUS: 'reservation-status',
    USER_RESERVATION_STATUS: 'user-reservation-status',
    GET_ACTIVE_RESERVED_GAME: 'get-active-reserved-game',
    GET_ACTIVE_RESERVED_GAMES: 'get-active-reserved-games',
    GET_ACTIVE_RESERVED_GAMES_COUNT: 'get-active-reserved-games-count',
    APPROVE_RESERVATION: 'approve-reservation',
    DECLINE_RESERVATION: 'decline-reservation',
    UPDATE_RESERVATION: 'update-reservation',
    DELETE_RESERVATION: 'delete-reservation',
    GET_RESERVATION_HISTORY: 'get-reservation-history',
    GET_RESERVATION_BY_ID: 'get-reservation',
  },
  GAME_REVIEWS: {
    CORE: 'gameReviews',
  },
  GAME_CATEGORIES: {
    CORE: 'gameCategories',
    LIST: 'gameCategories/list',
  },
  EVENTS: {
    CORE: 'events',
    LIST_FOR_USERS: 'get-list-for-user',
    LIST_FOR_STAFF: 'get-list-for-staff',
    GET_USER_EVENTS: 'get-user-events',
    PARTICIPATE: 'participate',
    REMOVE_PARTICIPANT: 'remove-participant',
    CHECK_USER_PARTICIPATION: 'check-user-participation',
    GET_ALL_EVENTS_DROPDOWN_LIST: 'get-all-events-dropdown-list',
  },
  ROOMS: {
    CORE: 'rooms',
    JOIN: 'join',
    LEAVE: 'leave',
    LIST: 'list',
    MEMBER_LIST: 'members',
    REMOVE_MEMBER: 'remove-member',
    MESSAGE_LIST: 'messages',
    INFO_MESSAGE_LIST: 'info-messages',
    CHECK_USER_PARTICIPATION: 'check-user-participation',
  },
  SCANNER: {
    CORE: 'scanner',
    UPLOAD: 'upload',
  },
  REWARDS: {
    CORE: 'rewards',
    SYSTEM_REWARD: 'system-reward',
    SYSTEM_REWARD_LIST: 'system-reward-list',
    GET_ALL_SYSTEM_REWARD_LIST: 'get-all-system-rewards-list',
    GET_USER_REWARDS: 'get-user-rewards',
    GET_USER_PERIOD_REWARDS: 'get-user-period-rewards',
    USER_REWARD_CONFIRMATION: 'user-reward-confirmation',
  },
  CHALLENGES: {
    CORE: 'challenges',
    LIST: 'list',
    GET_USER_CHALLENGES: 'get-user-challenges',
    GET_USER_CHALLENGE_PERIOD_PERFORMANCE:
      'get-user-challenge-period-performance',
  },
  SPACE_MANAGEMENT: {
    CORE: 'spaceManagement',
    JOIN: 'join',
    LEAVE_TABLE: 'leave-table',
    CLOSE_TABLE: 'close-table',
    REMOVE_USER_FROM_TABLE: 'remove-user-from-table',
    GET_SPACE_AVAILABLE_TABLES: 'get-space-available-tables',
    GET_USER_ACTIVE_TABLE: 'get-user-active-table',
    GET_SPACE_ACTIVITY_STATS: 'get-space-activity-stats',
    GET_TABLE_PARTICIPANTS: 'get-table-participants',
    ADD_VIRTUAL_PARTICIPANT: 'add-virtual-participant',
    REMOVE_VIRTUAL_PARTICIPANT: 'remove-virtual-participant',
    GET_ACTIVE_BOOKED_TABLE: 'get-active-booked-table',
    GET_ACTIVE_RESERVED_TABLES: 'get-active-reserved-tables',
    GET_ACTIVE_RESERVED_TABLES_COUNT: 'get-active-reserved-tables-count',
    GET_RESERVATION_HISTORY: 'get-reservation-history',
    BOOK_TABLE: 'book-table',
    GET_RESERVATION_BY_ID: 'get-reservation',
    APPROVE_RESERVATION: 'approve-reservation',
    DECLINE_RESERVATION: 'decline-reservation',
    UPDATE_RESERVATION: 'update-reservation',
    DELETE_RESERVATION: 'delete-reservation',
  },
  MESSAGING: {
    CORE: 'message',
  },
  TENANT_SETTINGS: {
    CORE: 'tenantSettings',
  },
  USER_SETTINGS: {
    CORE: 'tenantUserSettings',
    ASSISTIVE_TOUCH_SETTINGS: 'assistive-touch-settings',
  },
  USER: {
    CORE: 'user',
    SAVE_TOKEN: 'save-token',
    GET_EMPLOYEE_LIST: 'get-employee-list',
    GET_USER_LIST: 'get-user-list',
    CREATE_EMPLOYEE: 'create-employee',
    GET_USER_STATS: 'get-user-stats',
  },
  STATISTICS: {
    CORE: 'statistic',
    GET_ACTIVITY_CHART_DATA: 'get-activity-chart-data',
    GET_RESERVATION_CHART_DATA: 'get-reservation-chart-data',
    GET_EVENT_ATTENDANCE_CHART_DATA: 'get-event-attendance-chart-data',
    GET_EVENT_ATTENDANCE_BY_IDS: 'get-event-attendance-by-ids',
    GET_COLLECTED_REWARDS_BY_DATES: 'get-collected-rewards-by-dates',
    GET_EXPIRED_COLLECTED_REWARDS_CHART_DATA:
      'get-expired-collected-rewards-chart-data',
    GET_CHALLENGE_HISTORY_LOG: 'get-challenge-history-log',
  },
  // AUTH: {
  //   LOGIN: 'auth/user/login',
  // },
};
