import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-password-visibility-toggle',
  templateUrl: 'password-visibility-toggle.component.html',
  styleUrls: ['password-visibility-toggle.component.scss'],
})
export class PasswordVisibilityToggleComponent {
  @Input() isVisible: boolean = false;
  @Output() isVisibleChange = new EventEmitter<boolean>();

  public toggle(): void {
    this.isVisible = !this.isVisible;
    this.isVisibleChange.emit(this.isVisible);
  }
}
