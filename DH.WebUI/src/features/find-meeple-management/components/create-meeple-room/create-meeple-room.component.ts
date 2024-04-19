import {
  Component,
  ViewEncapsulation,
} from '@angular/core';

@Component({
  selector: 'app-create-meeple-room',
  templateUrl: 'create-meeple-room.component.html',
  styleUrl: 'create-meeple-room.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class CreateMeepleRoomComponent {
  cities = [
    { id: 1, name: 'Vilnius' },
    { id: 2, name: 'Kaunas' },
    { id: 3, name: 'Pavilnys' },
    { id: 4, name: 'Pabradė' },
    { id: 5, name: 'Klaipėda' },
    { id: 6, name: 'Vilnius' },
    { id: 7, name: 'Kaunas' },
    { id: 8, name: 'Pavilnys' },
    { id: 9, name: 'Pabradė' },
    { id: 11, name: 'Klaipėda' },
    { id: 12, name: 'Vilnius' },
    { id: 13, name: 'Kaunas' },
    { id: 14, name: 'Pavilnys' },
    { id: 15, name: 'Pabradė' },
    { id: 16, name: 'Klaipėda' },
  ];

  selectedCity: any;
}
