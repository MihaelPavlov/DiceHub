import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-btn',
  templateUrl: 'button.component.html',
  styleUrl: 'button.component.scss',
})
export class ButtonComponent {
  @Input({ required: true }) text!: string;
}
