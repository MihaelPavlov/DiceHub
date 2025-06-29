import {
  CanDeactivateFn,
} from '@angular/router';
import { CanComponentDeactivate } from './can-component-deactivate.interface';

export const canDeactivateGuard: CanDeactivateFn<CanComponentDeactivate> = (
  component
) => {
  return component.canDeactivate ? component.canDeactivate() : true;
};