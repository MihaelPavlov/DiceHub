import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-btn',
  templateUrl: 'button.component.html',
  styleUrl: 'button.component.scss',
})
export class ButtonComponent {
  @Input({ required: true }) text!: string;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() color: 'blue' | 'green' | 'red' | 'yellow' | 'orange' | 'gray' =
    'blue';
  @Input() fontColor!: 'blue' | 'green' | 'red' | 'yellow' | 'orange';
  @Input() borderColor!: 'blue' | 'green' | 'red' | 'yellow' | 'orange';

  getClasses() {
    let classes = ['btn'];
    classes.push(`btn-${this.size}`);
    if (this.color === 'blue') {
      classes.push('btn-blue');
    } else if (this.color === 'green') {
      classes.push('btn-green');
    } else if (this.color === 'red') {
      classes.push('btn-red');
    } else if (this.color === 'yellow') {
      classes.push('btn-yellow');
    } else if (this.color === 'orange') {
      classes.push('btn-orange');
    } else if (this.color === 'gray') {
      classes.push('btn-gray');
    }

    if (this.fontColor) {
      if (this.fontColor === 'blue') {
        classes.push('font-blue');
      } else if (this.fontColor === 'green') {
        classes.push('font-green');
      } else if (this.fontColor === 'red') {
        classes.push('font-red');
      } else if (this.fontColor === 'yellow') {
        classes.push('font-yellow');
      } else if (this.fontColor === 'orange') {
        classes.push('font-orange');
      }
    }

    if (this.borderColor) {
      if (this.borderColor === 'blue') {
        classes.push('border-blue');
      } else if (this.borderColor === 'green') {
        classes.push('border-green');
      } else if (this.borderColor === 'red') {
        classes.push('border-red');
      } else if (this.borderColor === 'yellow') {
        classes.push('border-yellow');
      } else if (this.borderColor === 'orange') {
        classes.push('border-orange');
      }
    }

    return classes.join(' ');
  }
}
