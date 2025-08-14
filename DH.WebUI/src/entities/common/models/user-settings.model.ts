import { SupportLanguages } from './support-languages.enum';

export interface IUserSettings {
  id?: number | null;
  phoneNumber: string;
  language: SupportLanguages;
}
