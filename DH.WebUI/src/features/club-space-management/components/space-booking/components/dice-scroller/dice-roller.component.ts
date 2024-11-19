import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-dice-roller',
  templateUrl: 'dice-roller.component.html',
  styleUrls: ['dice-roller.component.scss'],
})
export class DiceRollerComponent implements OnInit {
  currentFace: number = 1;
  isRolling: boolean = false;
  @Output() faceChange = new EventEmitter<number>(); // Emit current face to parent
  
  ngOnInit(): void {
    this.isRolling = true;

    // After animation duration (500ms), change the face
    setTimeout(() => {
      this.currentFace = 1

      this.faceChange.emit(this.currentFace); // Emit the updated value to the parent
      this.isRolling = false; // Stop the roll animation after the face has changed
    }, 500); // Duration of the roll animation
  }
  // Method to generate dot positions based on face number
  getDotPositions(face: number): { top: string; left: string }[] {
    const dotPositions = {
      1: [{ top: '50%', left: '50%' }],
      2: [
        { top: '25%', left: '25%' },
        { top: '75%', left: '75%' },
      ],
      3: [
        { top: '50%', left: '50%' },
        { top: '25%', left: '25%' },
        { top: '75%', left: '75%' },
      ],
      4: [
        { top: '25%', left: '25%' },
        { top: '25%', left: '75%' },
        { top: '75%', left: '25%' },
        { top: '75%', left: '75%' },
      ],
      5: [
        { top: '50%', left: '50%' },
        { top: '25%', left: '25%' },
        { top: '25%', left: '75%' },
        { top: '75%', left: '25%' },
        { top: '75%', left: '75%' },
      ],
      6: [
        { top: '25%', left: '25%' },
        { top: '25%', left: '50%' },
        { top: '25%', left: '75%' },
        { top: '75%', left: '25%' },
        { top: '75%', left: '50%' },
        { top: '75%', left: '75%' },
      ],
    };
    return dotPositions[face] || [];
  }

  // Toggle roll animation when left or right arrow is clicked
  changeFace(direction: 'left' | 'right'): void {
    this.isRolling = true;

    // After animation duration (500ms), change the face
    setTimeout(() => {
      this.currentFace =
        direction === 'left'
          ? this.currentFace === 1
            ? 6
            : this.currentFace - 1
          : this.currentFace === 6
          ? 1
          : this.currentFace + 1;

      this.faceChange.emit(this.currentFace); // Emit the updated value to the parent
      this.isRolling = false; // Stop the roll animation after the face has changed
    }, 500); // Duration of the roll animation
  }
}
