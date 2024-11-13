export const ROUTE = {
  LOGIN: 'login',
  REGISTER: 'register',
  GAMES: {
    CORE: 'games',
    LIBRARY: 'library',
    DETAILS: '/games/{id}/details',
    DETAILS_AVAILABILITY: '/games/{id}/availability',
    DETAILS_REVIEWS: '/games/{id}/reviews',
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
};

export const FULL_ROUTE = {
  GAMES: {
    LIBRARY: `${ROUTE.GAMES.CORE}/${ROUTE.GAMES.LIBRARY}`,
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
    UPDATE_BY_ID: (tableId: number) => `${ROUTE.SPACE_MANAGEMENT.CORE}/${ROUTE.SPACE_MANAGEMENT.UPDATE}/${tableId}`,
  },
};
