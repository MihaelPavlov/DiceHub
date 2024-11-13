import { Observable } from 'rxjs';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IMenuItem } from '../../models/menu-item.model';

@Component({
  selector: 'app-controls-menu',
  templateUrl: 'controls-menu.component.html',
  styleUrls: ['controls-menu.component.scss'],
})
export class ControlsMenuComponent {
  @Input() isVisible: boolean = false;
  @Input() menuItems!: Observable<IMenuItem[]>;

  @Output() menuItemClick = new EventEmitter<string>();
  @Output() visibilityChange = new EventEmitter<boolean>();

  public onMenuItemClick(key: string): void {
    this.menuItemClick.emit(key);
    this.closeMenu();
  }

  public closeMenu(): void {
    this.isVisible = false;
    this.visibilityChange.emit(this.isVisible); // Notify parent if needed
  }

  public toggleMenu(): void {
    this.isVisible = !this.isVisible;
    this.visibilityChange.emit(this.isVisible);
  }
}
