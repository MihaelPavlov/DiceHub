import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-chip',
  templateUrl: 'chip.component.html',
  styleUrl: 'chip.component.scss',
})
export class ChipComponent implements OnInit {
  @Input() size: 'small' | 'middle' | 'big' = 'middle';
  @Input({ required: true }) color!: 'red' | 'blue' | 'orange' | 'yellow';
  @Input({ required: true }) text!: string;

  public systemColor!: string;

  public chipBlueColor: string = '#21419b';
  public chipYellowColor: string = '#977f33';
  public chipOrangeColor: string = '#98542b';
  public chipRedColor: string = '#853f47';

  public ngOnInit(): void {
    switch (this.color) {
      case 'red':
        this.systemColor = this.chipRedColor;
        break;
      case 'blue':
        this.systemColor = this.chipBlueColor;
        break;
      case 'orange':
        this.systemColor = this.chipOrangeColor;
        break;
      case 'yellow':
        this.systemColor = this.chipYellowColor;
        break;
    }
  }
}
