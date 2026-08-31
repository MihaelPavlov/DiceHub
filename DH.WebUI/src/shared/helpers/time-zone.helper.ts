export const DEFAULT_TIME_ZONE_ID = 'Europe/Sofia';

// Used only when the browser doesn't expose Intl.supportedValuesOf('timeZone')
// (older WebViews). Europe-heavy because that's where the clubs are, plus a
// spread of common zones worldwide.
const TIME_ZONE_FALLBACK: string[] = [
  'Europe/Sofia',
  'Europe/Athens',
  'Europe/Bucharest',
  'Europe/Belgrade',
  'Europe/Berlin',
  'Europe/Madrid',
  'Europe/Paris',
  'Europe/Rome',
  'Europe/Amsterdam',
  'Europe/Vienna',
  'Europe/Warsaw',
  'Europe/Prague',
  'Europe/Budapest',
  'Europe/Zurich',
  'Europe/London',
  'Europe/Lisbon',
  'Europe/Dublin',
  'Europe/Helsinki',
  'Europe/Kyiv',
  'Europe/Istanbul',
  'Europe/Moscow',
  'Atlantic/Reykjavik',
  'America/New_York',
  'America/Chicago',
  'America/Denver',
  'America/Los_Angeles',
  'America/Sao_Paulo',
  'Asia/Dubai',
  'Asia/Jerusalem',
  'Asia/Kolkata',
  'Asia/Singapore',
  'Asia/Tokyo',
  'Australia/Sydney',
  'UTC',
];

/** The full IANA zone list when the browser supports it, otherwise a curated subset. */
export function getSupportedTimeZones(): string[] {
  const supportedValuesOf = (Intl as unknown as {
    supportedValuesOf?: (key: string) => string[];
  }).supportedValuesOf;

  const zones =
    typeof supportedValuesOf === 'function'
      ? supportedValuesOf('timeZone')
      : TIME_ZONE_FALLBACK;

  return zones && zones.length ? zones : TIME_ZONE_FALLBACK;
}

/** The viewer's own time zone, e.g. "Europe/Sofia"; falls back to the default. */
export function detectBrowserTimeZone(): string {
  try {
    return (
      Intl.DateTimeFormat().resolvedOptions().timeZone || DEFAULT_TIME_ZONE_ID
    );
  } catch {
    return DEFAULT_TIME_ZONE_ID;
  }
}
