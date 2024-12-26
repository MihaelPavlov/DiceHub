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

export const colors: Colors = {
  blue: {
    default: 'rgba(55, 156, 226, 1)', // #379ce2 in RGBA
    half: 'rgba(55, 156, 226, 0.5)', // Semi-transparent version of #379ce2
    quarter: 'rgba(55, 156, 226, 0.25)', // More transparent version
    zero: 'rgba(55, 156, 226, 0)', // Fully transparent
  },
  yellow: {
    default: 'rgba(240, 200, 8, 1)', // #F0C808 in RGBA
    half: 'rgba(240, 200, 8, 0.5)', // Semi-transparent version of #379ce2
    quarter: 'rgba(240, 200, 8, 0.25)', // More transparent version
    zero: 'rgba(240, 200, 8, 0)', // Fully transparent
  },
  green: {
    default: 'rgba(82, 203, 164, 1)', // #52cba4 in RGBA
    half: 'rgba(82, 203, 164, 0.5)', // Semi-transparent version of #52cba4
    quarter: 'rgba(82, 203, 164, 0.25)', // More transparent version
    zero: 'rgba(82, 203, 164, 0)', // Fully transparent
  },
  green2: {
    default: 'rgba(74, 201, 95, 1)', // #4ac95f in RGBA
    half: 'rgba(74, 201, 95, 0.5)', // Semi-transparent version of #4ac95f
    quarter: 'rgba(74, 201, 95, 0.25)', // More transparent version
    zero: 'rgba(74, 201, 95, 0)', // Fully transparent
  },
  indigo: {
    default: 'rgba(55, 95, 131, 1)', // A complementary indigo color closer to #379ce2
    quarter: 'rgba(55, 95, 131, 0.25)', // Semi-transparent indigo
  },
  purple: {
    default: 'rgba(210, 112, 212, 1)', // #d270d4 in RGBA
    half: 'rgba(210, 112, 212, 0.5)', // Semi-transparent version of #d270d4
    quarter: 'rgba(210, 112, 212, 0.25)', // More transparent version
    zero: 'rgba(210, 112, 212, 0)', // Fully transparent
  },
  peach: {
    default: 'rgba(255, 152, 91, 1)', // #ff985b in RGBA
    half: 'rgba(255, 152, 91, 0.5)', // Semi-transparent version of #ff985b
    quarter: 'rgba(255, 152, 91, 0.25)', // More transparent version
    zero: 'rgba(255, 152, 91, 0)', // Fully transparent
  },
  purple2: {
    default: 'rgba(164, 96, 226, 1)', // #a460e2 in RGBA
    half: 'rgba(164, 96, 226, 0.5)', // Semi-transparent version of #a460e2
    quarter: 'rgba(164, 96, 226, 0.25)', // More transparent version
    zero: 'rgba(164, 96, 226, 0)', // Fully transparent
  },
  pinkPurple: {
    default: 'rgba(175, 73, 130, 1)', // #af4982 in RGBA
    half: 'rgba(175, 73, 130, 0.5)', // Semi-transparent version of #af4982
    quarter: 'rgba(175, 73, 130, 0.25)', // More transparent version
    zero: 'rgba(175, 73, 130, 0)', // Fully transparent
  },
  fadedPeach: {
    default: 'rgba(246, 172, 133, 1)', // #f6ac85 in RGBA
    half: 'rgba(246, 172, 133, 0.5)', // Semi-transparent version of #f6ac85
    quarter: 'rgba(246, 172, 133, 0.25)', // More transparent version
    zero: 'rgba(246, 172, 133, 0)', // Fully transparent
  },
};
