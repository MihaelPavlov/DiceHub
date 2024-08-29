import { FormControl, FormGroup } from '@angular/forms';

export type Formify<T> = FormGroup<{
  [K in keyof T]: FormControl<T[K]>;
}>;
