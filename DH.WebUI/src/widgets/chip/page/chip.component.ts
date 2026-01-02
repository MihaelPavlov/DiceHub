import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-chip',
  templateUrl: 'chip.component.html',
  styleUrl: 'chip.component.scss',
  standalone: false,
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

  public systemColorCssName!: string;
  private readonly colorClassMap: Record<string, string> = {
    red: 'chipRedColor',
    blue: 'chipBlueColor',
    orange: 'chipOrangeColor',
    yellow: 'chipYellowColor',
    green: 'chipGreenColor',
  };

  public ngOnInit(): void {
    this.systemColorCssName = this.colorClassMap[this.color];
  }
}
