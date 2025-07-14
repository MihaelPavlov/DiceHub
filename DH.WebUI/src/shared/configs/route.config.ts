export const ROUTE = {
  LANDING: '/',
  LOGIN: 'login',
  REGISTER: 'register',
  FORGOT_PASSWORD: 'forgot-password',
  INSTRUCTIONS: 'instructions',
  GAMES: {
    CORE: 'games',
    LIBRARY: 'library',
    DETAILS: 'details',
    AVAILABILITY: 'availability',
    REVIEWS: 'reviews',
    UPDATE: 'update',
    ADD_EXISTING_GAME: 'add-existing-game',
    RESERVATIONS: 'reservations',
  },
  EVENTS: {
    CORE: 'events',
    HOME: 'home',
    DETAILS: `details`,
    ADMIN: {
      CORE: 'admin',
      ADD: 'add',
      UPDATE: 'update',
      DETAILS: 'details',
    },
  },
  CHALLENGES: {
    CORE: 'challenges',
    HOME: 'home',
    REWARDS: 'rewards',
    ADMIN: {
      CORE: 'admin',
      REWARDS: 'rewards',
      LIST: 'list',
      SETTINGS: 'settings',
      SYSTEM_REWARDS: 'system-rewards',
      HISTORY_LOG: 'history-log',
      CUSTOM_PERIOD: 'custom-period',
    },
  },
  SPACE_MANAGEMENT: {
    CORE: 'space',
    HOME: 'home',
    DETAILS: 'details',
    CREATE: 'create',
    UPDATE: 'update',
    RESERVATIONS: 'reservations',
    TABLES: 'tables',
  },
  CHAT_HUB_CLIENT: {
    CORE: 'chatHub',
    SEND_MESSAGE_TO_GROUP: 'SendMessageToGroup',
    RECEIVE_MESSAGE: 'ReceiveMessage',
    CONNECT_TO_GROUP: 'ConnectToGroup',
  },
  MEEPLE_ROOMS: {
    CORE: 'meeples',
    CREATE: 'create',
    DETAILS: 'details',
    FIND: 'find',
    CHAT: 'chat',
  },
  QR_CODE_SCANNER: 'qr-code-scanner',
  PROFILE: {
    CORE: 'profile',
    EMPLOYEES: 'employees',
    ADD_EMPLOYEE: 'add-employee',
    UPDATE_EMPLOYEE: 'update-employee',
    OWNER_DETAILS: 'owner-details',
  },
};

export const FULL_ROUTE = {
  GAMES: {
    LIBRARY: `${ROUTE.GAMES.CORE}/${ROUTE.GAMES.LIBRARY}`,
    LIBRARY_BY_CATEGORY_ID: (categoryId: number) =>
      `${ROUTE.GAMES.CORE}/${ROUTE.GAMES.LIBRARY}/${categoryId}`,
    DETAILS: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.DETAILS}`,
    AVAILABILITY: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.AVAILABILITY}`,
    REVIEWS: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.REVIEWS}`,
    UPDATE: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.UPDATE}`,
    ADD_EXISTING_GAME: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.ADD_EXISTING_GAME}`,
    ACTIVE_RESERVATIONS: `${ROUTE.GAMES.RESERVATIONS}/${ROUTE.GAMES.CORE}`,
  },
  EVENTS: {
    HOME: `${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.HOME}`,
    DETAILS_BY_ID: (eventId: number) =>
      `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.DETAILS}`,
    ADMIN: {
      CORE: `${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.ADMIN.CORE}`,
      ADD: `${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.ADD}`,
      UPDATE_BY_ID: (eventId: number) =>
        `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.UPDATE}`,
      DETAILS_BY_ID: (eventId: number) =>
        `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.DETAILS}`,
    },
  },
  CHALLENGES: {
    CHALLENGES_HOME: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.HOME}`,
    CHALLENGES_REWARDS: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.REWARDS}`,
    ADMIN_LIST: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.LIST}`,
    ADMIN_SETTINGS: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.SETTINGS}`,
    ADMIN_REWARDS: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.REWARDS}`,
    ADMIN_SYSTEM_REWARDS: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.SYSTEM_REWARDS}`,
    ADMIN_HISTORY_LOG: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.HISTORY_LOG}`,
    ADMIN_CUSTOM_PERIOD: `${ROUTE.CHALLENGES.CORE}/${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.CUSTOM_PERIOD}`,
  },
  SPACE_MANAGEMENT: {
    HOME: `${ROUTE.SPACE_MANAGEMENT.CORE}/${ROUTE.SPACE_MANAGEMENT.HOME}`,
    ACTIVE_RESERVATIONS: `${ROUTE.SPACE_MANAGEMENT.RESERVATIONS}/${ROUTE.SPACE_MANAGEMENT.TABLES}`,
    UPDATE_BY_ID: (tableId: number) =>
      `${ROUTE.SPACE_MANAGEMENT.CORE}/${ROUTE.SPACE_MANAGEMENT.UPDATE}/${tableId}`,
    ROOM_DETAILS: (tableId: number) =>
      `${ROUTE.SPACE_MANAGEMENT.CORE}/${tableId}/${ROUTE.SPACE_MANAGEMENT.DETAILS}`,
  },
  MEEPLE_ROOM: {
    DETAILS_BY_ID: (roomId: number) =>
      `${ROUTE.MEEPLE_ROOMS.CORE}/${roomId}/${ROUTE.MEEPLE_ROOMS.DETAILS}`,
    FIND: `${ROUTE.MEEPLE_ROOMS.CORE}/${ROUTE.MEEPLE_ROOMS.FIND}`,
    CHAT_ROOM_BY_ID: (roomId: number) =>
      `${ROUTE.MEEPLE_ROOMS.CORE}/${roomId}/${ROUTE.MEEPLE_ROOMS.CHAT}`,
    CREATE: `${ROUTE.MEEPLE_ROOMS.CORE}/${ROUTE.MEEPLE_ROOMS.CREATE}`,
  },
  PROFILE: {
    EMPLOYEES: `${ROUTE.PROFILE.CORE}/${ROUTE.PROFILE.EMPLOYEES}`,
    ADD_EMPLOYEE: `${ROUTE.PROFILE.CORE}/${ROUTE.PROFILE.ADD_EMPLOYEE}`,
    UPDATE_BY_ID: (employeeId: string) =>
      `${ROUTE.PROFILE.CORE}/${employeeId}/${ROUTE.PROFILE.UPDATE_EMPLOYEE}`,
    OWNER_DETAILS: `${ROUTE.PROFILE.CORE}/${ROUTE.PROFILE.OWNER_DETAILS}`,
  },
};
