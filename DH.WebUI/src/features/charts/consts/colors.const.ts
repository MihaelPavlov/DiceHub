import { ColorShades } from '../models/color-shades.model';

export interface Colors {
  blue: ColorShades;
  yellow: ColorShades;
  green: ColorShades;
  green2: ColorShades;
  indigo: ColorShades;
  purple: ColorShades;
  peach: ColorShades;
  purple2: ColorShades;
  pinkPurple: ColorShades;
  fadedPeach: ColorShades;
}

// Reimagined design-system palette (amber / coral / teal / violet family),
// re-mapped onto the existing chart.js dataset color keys so every chart
// consumer keeps working without touching each component's .ts.
export const colors: Colors = {
  blue: {
    // reimagined violet — visitors chart, weekly series
    default: 'rgba(141, 102, 214, 1)', // #8d66d6
    half: 'rgba(141, 102, 214, 0.5)',
    quarter: 'rgba(141, 102, 214, 0.25)',
    zero: 'rgba(141, 102, 214, 0)',
  },
  yellow: {
    // reimagined amber — visitors chart, yearly series
    default: 'rgba(243, 191, 61, 1)', // #f3bf3d
    half: 'rgba(243, 191, 61, 0.5)',
    quarter: 'rgba(243, 191, 61, 0.25)',
    zero: 'rgba(243, 191, 61, 0)',
  },
  green: {
    // reimagined teal — visitors chart, monthly series
    default: 'rgba(94, 199, 162, 1)', // #5ec7a2
    half: 'rgba(94, 199, 162, 0.5)',
    quarter: 'rgba(94, 199, 162, 0.25)',
    zero: 'rgba(94, 199, 162, 0)',
  },
  green2: {
    // reimagined coral — reservations chart, cancelled series
    default: 'rgba(191, 60, 101, 1)', // #bf3c65
    half: 'rgba(191, 60, 101, 0.5)',
    quarter: 'rgba(191, 60, 101, 0.25)',
    zero: 'rgba(191, 60, 101, 0)',
  },
  indigo: {
    // reimagined deep violet
    default: 'rgba(107, 79, 176, 1)', // #6b4fb0
    quarter: 'rgba(107, 79, 176, 0.25)',
  },
  purple: {
    // reimagined violet — reservations chart, completed series
    default: 'rgba(141, 102, 214, 1)', // #8d66d6
    half: 'rgba(141, 102, 214, 0.5)',
    quarter: 'rgba(141, 102, 214, 0.25)',
    zero: 'rgba(141, 102, 214, 0)',
  },
  peach: {
    // reimagined amber — collected/expired rewards chart, collected series
    default: 'rgba(243, 191, 61, 1)', // #f3bf3d
    half: 'rgba(243, 191, 61, 0.5)',
    quarter: 'rgba(243, 191, 61, 0.25)',
    zero: 'rgba(243, 191, 61, 0)',
  },
  purple2: {
    // reimagined coral — collected/expired rewards chart, expired series
    default: 'rgba(191, 60, 101, 1)', // #bf3c65
    half: 'rgba(191, 60, 101, 0.5)',
    quarter: 'rgba(191, 60, 101, 0.25)',
    zero: 'rgba(191, 60, 101, 0)',
  },
  pinkPurple: {
    // reimagined deep coral
    default: 'rgba(156, 47, 79, 1)', // #9c2f4f
    half: 'rgba(156, 47, 79, 0.5)',
    quarter: 'rgba(156, 47, 79, 0.25)',
    zero: 'rgba(156, 47, 79, 0)',
  },
  fadedPeach: {
    // reimagined light amber — rewards collected chart (single series)
    default: 'rgba(247, 207, 112, 1)', // #f7cf70
    half: 'rgba(247, 207, 112, 0.5)',
    quarter: 'rgba(247, 207, 112, 0.25)',
    zero: 'rgba(247, 207, 112, 0)',
  },
};
