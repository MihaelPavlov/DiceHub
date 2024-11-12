import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IMenuItem } from '../../models/menu-item.model';

@Component({
  selector: 'app-controls-menu',
  templateUrl: 'controls-menu.component.html',
  styleUrls: ['controls-menu.component.scss'],
})
export class ControlsMenuComponent {
  @Input() isVisible: boolean = false; // Controls visibility of menu and overlay
  @Input() menuItems: IMenuItem[] = [];
  @Output() menuItemClick = new EventEmitter<string>();
  @Output() visibilityChange = new EventEmitter<boolean>();

  onMenuItemClick(key: string) {
    this.menuItemClick.emit(key);
    this.closeMenu();
  }

  closeMenu() {
    this.isVisible = false;
    this.visibilityChange.emit(this.isVisible); // Notify parent if needed
  }

  toggleMenu() {
    this.isVisible = !this.isVisible;
    this.visibilityChange.emit(this.isVisible);
  }
}
