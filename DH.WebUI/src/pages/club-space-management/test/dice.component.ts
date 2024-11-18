import { Component } from '@angular/core';

@Component({
  selector: 'app-dice-roller',
  templateUrl: 'dice.component.html',
  styleUrls: ['dice.component.scss']
})
export class DiceRollerComponent {
    // result1: number = 1;
    // result2: number = 6;
    // sum: string = 'Sum: 7';
    // isRolling: boolean = false;

    // rollSingleDie(): number {
    //   return Math.floor(Math.random() * 6) + 1;
    // }

    // rollDice(): void {
    //   this.isRolling = true;

    //   setTimeout(() => {
    //     this.result1 = this.rollSingleDie();
    //     this.result2 = this.rollSingleDie();
    //     this.sum = 'Sum: ' + (this.result1 + this.result2);
    //     this.isRolling = false;
    //   }, 500); // Animation duration
    // }

    // // Returns an array of dot positions for each dice face
    // getDotPositions(face: number): Array<{ top: string, left: string }> {
    //   const positions = {
    //     1: [{ top: '50%', left: '50%' }],
    //     2: [
    //       { top: '25%', left: '25%' },
    //       { top: '75%', left: '75%' }
    //     ],
    //     3: [
    //       { top: '50%', left: '50%' },
    //       { top: '25%', left: '25%' },
    //       { top: '75%', left: '75%' }
    //     ],
    //     4: [
    //       { top: '25%', left: '25%' },
    //       { top: '25%', left: '75%' },
    //       { top: '75%', left: '25%' },
    //       { top: '75%', left: '75%' }
    //     ],
    //     5: [
    //       { top: '50%', left: '50%' },
    //       { top: '25%', left: '25%' },
    //       { top: '25%', left: '75%' },
    //       { top: '75%', left: '25%' },
    //       { top: '75%', left: '75%' }
    //     ],
    //     6: [
    //       { top: '25%', left: '25%' },
    //       { top: '25%', left: '50%' },
    //       { top: '25%', left: '75%' },
    //       { top: '75%', left: '25%' },
    //       { top: '75%', left: '50%' },
    //       { top: '75%', left: '75%' }
    //     ]
    //   };
    //   return positions[face] || [];
    // }

    currentFace: number = 1;
    isRolling: boolean = false;
  
    // Method to generate dot positions based on face number
    getDotPositions(face: number): { top: string; left: string }[] {
      const dotPositions = {
        1: [{ top: '50%', left: '50%' }],
        2: [{ top: '25%', left: '25%' }, { top: '75%', left: '75%' }],
        3: [
          { top: '50%', left: '50%' },
          { top: '25%', left: '25%' },
          { top: '75%', left: '75%' }
        ],
        4: [
          { top: '25%', left: '25%' },
          { top: '25%', left: '75%' },
          { top: '75%', left: '25%' },
          { top: '75%', left: '75%' }
        ],
        5: [
          { top: '50%', left: '50%' },
          { top: '25%', left: '25%' },
          { top: '25%', left: '75%' },
          { top: '75%', left: '25%' },
          { top: '75%', left: '75%' }
        ],
        6: [
          { top: '25%', left: '25%' },
          { top: '25%', left: '50%' },
          { top: '25%', left: '75%' },
          { top: '75%', left: '25%' },
          { top: '75%', left: '50%' },
          { top: '75%', left: '75%' }
        ]
      };
      return dotPositions[face] || [];
    }
  
    // Toggle roll animation when left or right arrow is clicked
    changeFace(direction: 'left' | 'right'): void {
      this.isRolling = true;
  
      // After animation duration (500ms), change the face
      setTimeout(() => {
        this.currentFace = direction === 'left'
          ? (this.currentFace === 1 ? 6 : this.currentFace - 1)
          : (this.currentFace === 6 ? 1 : this.currentFace + 1);
  
        this.isRolling = false; // Stop the roll animation after the face has changed
      }, 500); // Duration of the roll animation
    }
}
