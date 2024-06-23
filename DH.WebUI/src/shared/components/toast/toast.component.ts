import { ChangeDetectionStrategy, Component, Inject, ViewEncapsulation } from "@angular/core";
import { IToast } from "../../models/toast.model";
import * as snackBar from "@angular/material/snack-bar";

@Component({
    selector: 'app-toast',
    templateUrl: 'toast.component.html',
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush,
  })
  export class ToastComponent {
    constructor(
      @Inject(snackBar.MAT_SNACK_BAR_DATA) public toastData: IToast,
      public snackBarRef: snackBar.MatSnackBarRef<ToastComponent>
    ) {}
  
    public getIconPathByType(type: string): string {
      if (type) {
        return `assets/img/${type}-toast-icon.svg`;
      }
      return '';
    }
  }