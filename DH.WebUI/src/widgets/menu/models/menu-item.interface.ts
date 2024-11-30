export interface IMenuItemInterface {
  label: string;
  class: string;
  enabled: boolean;
  isAlertActive: boolean;
  visible: boolean;
  icon: string;
  route: string;
  translatable?: boolean;
  forceActive?: boolean;
}
