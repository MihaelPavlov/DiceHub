import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-challenges-management',
  templateUrl: 'challenges-management.component.html',
  styleUrl: 'challenges-management.component.scss',
})
export class ChallengesManagementComponent implements OnInit {
  constructor(private readonly router: Router) {}
  ngOnInit(): void {
    const barValue = document.querySelector('.bar-2-value') as HTMLElement;

    // Modify the keyframes dynamically
    const style = document.createElement('style');
    style.textContent = `
        @keyframes custom-load {
            0% {
                width: 0;
            }
    
            100% {
                width: 50%; // Modify this value as needed
            }
        }
    `;

    // Append the style to the document head
    document.head.appendChild(style);

    // Apply the modified animation to the bar value element
    barValue.style.animation = 'custom-load 3s normal forwards';

    // Top challenge bar with point
    const progress: HTMLElement | null = document.getElementById('progress');
    const stepCircles: NodeListOf<Element> =
      document.querySelectorAll('.circle');
    let currentActive: number = 2;

    // NOTE CHANGE HERE TO 1-4
    // 1=25%
    // 2=50%
    // 3=75%
    // 4=100%
    update(currentActive);

    function update(currentActive: number): void {
      stepCircles.forEach((circle: Element, i: number) => {
        if (i < currentActive) {
          circle.classList.add('activated');
          circle.innerHTML = `
          <img class="_img"
          src="../../../shared/assets/images/challenge-complete.png"
          alt=""
          />`;
          circle.classList.add('_img');
        } else {
          circle.classList.remove('activated');
        }
      });

      const activeCircles: NodeListOf<Element> =
        document.querySelectorAll('.activated');
      console.log(activeCircles);
      if (progress && stepCircles.length > 1) {
        console.log(
          ((activeCircles.length - 1) / (stepCircles.length - 1)) * 100 + '%'
        );

        progress.style.width =
          ((activeCircles.length - 1) / (stepCircles.length - 1)) * 100 + '%';
      }
    }
  }
  //   ngAfterViewInit(): void {
  //     const progress: HTMLElement | null = document.getElementById("progress");
  // const stepCircles: NodeListOf<Element> = document.querySelectorAll(".circle");
  // let currentActive: number = 1;

  // // NOTE CHANGE HERE TO 1-4
  // // 1=25%
  // // 2=50%
  // // 3=75%
  // // 4=100%
  // update(3);

  // function update(currentActive: number): void {
  //   stepCircles.forEach((circle: Element, i: number) => {
  //     if (i < currentActive) {
  //       circle.classList.add("active");

  //       // circle.classList.add("_img");
  //       // circle.innerHTML = `
  //       // <img class="_img"
  //       // src="https://static.vecteezy.com/system/resources/thumbnails/017/350/123/small/green-check-mark-icon-in-round-shape-design-png.png"
  //       // alt=""
  //       // />`
  //     } else {
  //       circle.classList.remove("active");
  //     }
  //   });
  //   const activeCircles: NodeListOf<Element> = document.querySelectorAll(".active");
  //   if (progress && stepCircles.length > 1) {
  //     progress.style.width = ((activeCircles.length - 1) / (stepCircles.length - 1)) * 100 + "%";
  //   }
  // }
  //   }
}
