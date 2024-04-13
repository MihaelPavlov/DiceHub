import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-menu',
  templateUrl: 'menu.component.html',
  styleUrl: 'menu.component.scss',
})
export class MenuComponent implements OnInit {
  public ngOnInit(): void {
    this.onInitJS();
  }

  private onInitJS(): void {
    document.addEventListener('DOMContentLoaded', function () {
      const interactiveOption = document.querySelector(
        '.interactive-option'
      ) as HTMLElement;

      interactiveOption.addEventListener('click', function (event) {
        event.stopPropagation(); // Prevents the click event from propagating to the document body

        interactiveOption.classList.toggle('active');
      });

      // Hide the interactive option when clicking anywhere outside of it
      document.documentElement.addEventListener('mousedown', function (event) {
        if (!interactiveOption.contains(event.target as Node)) {
          interactiveOption.classList.remove('active');
        }
      });

    });
  }
}
