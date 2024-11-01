export const ROUTE = {
  GAMES: {
    CORE: '/games',
    LIBRARY: '/games/libary',
    DETAILS: '/games/{id}/details',
    DETAILS_AVAILABILITY: '/games/{id}/availability',
    DETAILS_REVIEWS: '/games/{id}/reviews',
  },
  EVENTS: {
    CORE: 'events',
    HOME: 'home',
    DETAILS: `details`,
    ADMIN: {
      CORE: "admin",
      ADD: 'add',
      UPDATE: 'update',
      DETAILS: "details"
    },
  },
};

export const FULL_ROUTE= {
  EVENTS:{
    HOME: `${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.HOME}`,
    DETAILS_BY_ID: (eventId: number)=> `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.DETAILS}`,
    ADMIN: {
      CORE:`${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.ADMIN.CORE}`,
      ADD: `${ROUTE.EVENTS.CORE}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.ADD}`,
      UPDATE_BY_ID: (eventId: number)=> `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.UPDATE}`, 
      DETAILS_BY_ID: (eventId: number)=> `${ROUTE.EVENTS.CORE}/${eventId}/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.DETAILS}`
    } 
  }
}