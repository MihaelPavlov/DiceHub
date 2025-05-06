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
  @Input() isInfo: boolean = false;
  @Input() infoDescription: string = '';

  @Output() menuItemClick = new EventEmitter<{
    value: string;
    event: MouseEvent;
  }>();
  @Output() visibilityChange = new EventEmitter<boolean>();

  public   get infoDescriptionWithBreaks(): string {
    return this.infoDescription.replaceAll('\\r\\n', '<br />');
  }

  public onMenuItemClick(key: string, event: MouseEvent): void {
    this.menuItemClick.emit({ value: key, event });
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
