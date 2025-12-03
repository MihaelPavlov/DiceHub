import { Directive, ElementRef, Input, OnInit } from '@angular/core';

@Directive({
    selector: '[randomColor]',
    standalone: false
})
export class RandomColorDirective implements OnInit {
  @Input() randomColor: number = 1; // Optional index for specific hues

  constructor(private el: ElementRef) {}

  ngOnInit(): void {
    const color = this.generateColor(this.randomColor);
    this.el.nativeElement.style.color = color.default; // Apply the generated color
  }

  private generateColor(index?: number): {
    default: string;
    half: string;
    quarter: string;
    zero: string;
  } {
    const getRandomHue = () => Math.floor(Math.random() * 360); // Generate random hue
    const hues = [100, 120, 200, 360, 180, 100, 300, 180]; // Predefined hues
    const hue = index !== undefined ? hues[index % hues.length] : getRandomHue(); // Use index or random hue
    const saturation = 40;
    const lightness = 60;

    const toRgba = (alpha: number) =>
      `hsla(${hue}, ${saturation}%, ${lightness}%, ${alpha})`;

    return {
      default: toRgba(1),
      half: toRgba(0.5),
      quarter: toRgba(0.25),
      zero: toRgba(0),
    };
  }
}
