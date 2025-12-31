import { UiTheme } from '../../../shared/enums/ui-theme.enum';
import { SupportLanguages } from './support-languages.enum';

export interface IUserSettings {
  id?: number | null;
  phoneNumber: string;
  language: SupportLanguages;
  uiTheme: UiTheme;
}
