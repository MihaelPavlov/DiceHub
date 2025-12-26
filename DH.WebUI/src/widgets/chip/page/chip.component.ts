import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'app-chip',
    templateUrl: 'chip.component.html',
    styleUrl: 'chip.component.scss',
    standalone: false
})
export class ChipComponent implements OnInit {
  @Input() size: 'extraSmall' | 'small' | 'middle' | 'big' = 'middle';
  @Input({ required: true }) color!:
    | 'red'
    | 'blue'
    | 'orange'
    | 'yellow'
    | 'green';
  @Input({ required: true }) text!: string;

  public systemColor!: string;

  public chipBlueColor: string = '#4988C4';
  public chipYellowColor: string = '#FFD41D';
  public chipOrangeColor: string = '#DE802B';
  public chipRedColor: string = '#DE1A58';
  public chipGreenColor: string = '#A3D78A';
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
      case 'green':
        this.systemColor = this.chipGreenColor;
        break;
    }
  }
}
