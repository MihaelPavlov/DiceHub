import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-btn',
  templateUrl: 'button.component.html',
  styleUrl: 'button.component.scss',
})
export class ButtonComponent {
  @Input({ required: true }) text!: string;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() color: 'blue' | 'green' | 'red' = 'blue';

  getClasses() {
    let classes = ['btn'];
    classes.push(`btn-${this.size}`);
    if (this.color === 'blue') {
      classes.push('btn-blue');
    } else if (this.color === 'green') {
      classes.push('btn-green');
    } else if (this.color === 'red') {
      // Custom color handling
      classes.push('btn-red');
    }
    return classes.join(' ');
  }
}
