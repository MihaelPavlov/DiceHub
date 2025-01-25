export const ROUTE = {
  LOGIN: 'login',
  REGISTER: 'register',
  GAMES: {
    CORE: 'games',
    LIBRARY: 'library',
    DETAILS: 'details',
    AVAILABILITY: 'availability',
    REVIEWS: 'reviews',
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
    },
  },
  SPACE_MANAGEMENT: {
    CORE: 'space',
    HOME: 'home',
    DETAILS: 'details',
    CREATE: 'create',
    UPDATE: 'update',
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
};

export const FULL_ROUTE = {
  GAMES: {
    LIBRARY: `${ROUTE.GAMES.CORE}/${ROUTE.GAMES.LIBRARY}`,
    DETAILS: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.DETAILS}`,
    AVAILABILITY: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.AVAILABILITY}`,
    REVIEWS: (gameId: number) =>
      `/${ROUTE.GAMES.CORE}/${gameId}/${ROUTE.GAMES.REVIEWS}`,
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
  },
  SPACE_MANAGEMENT: {
    HOME: `${ROUTE.SPACE_MANAGEMENT.CORE}/${ROUTE.SPACE_MANAGEMENT.HOME}`,
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
};
