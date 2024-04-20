import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  styleUrl: 'header.component.scss',
})
export class HeaderComponent implements AfterViewInit {
  @Input() header!: string;
  @Input() withSearch: boolean = false;
  @Input() withAdd: boolean = false;
  @Input() withQRcode: boolean = false;
  @Input() withBottomLine: boolean = false;
  @Output() addClicked: EventEmitter<void> = new EventEmitter<void>();

  public ngAfterViewInit(): void {
    this.initSearchListenersJS();
  }

  public onAddButtonClick(): void {
    this.addClicked.emit();
  }

  private initSearchListenersJS(): void {
    const navbar = document.getElementById('navbar');
    let prevScrollPos = window.pageYOffset;
    if (navbar) {
      window.onscroll = function () {
        const currentScrollPos = window.pageYOffset;
        if (prevScrollPos > currentScrollPos) {
          navbar.classList.remove('hidden');
        } else {
          navbar.classList.add('hidden');
        }
        prevScrollPos = currentScrollPos;
      };
    }

    // Hide the search form initially
    const searchForm = document.getElementById('searchForm') as HTMLElement;

    if (searchForm) {
      searchForm.style.display = 'none';
    }

    // Add click event listener to the wrapper_search link
    const wrapperSearch =
      document.querySelector<HTMLElement>('.wrapper_search');
    if (wrapperSearch) {
      wrapperSearch.addEventListener('click', function () {
        // Toggle visibility of the search form
        if (searchForm.style.display === 'none') {
          searchForm.style.display = 'block';
          const searchIcon = document.querySelector<HTMLElement>('#search-btn');
          if (searchIcon) {
            searchIcon.innerHTML = `<span class="material-symbols-outlined">close</span>`;
          }
        } else {
          searchForm.style.display = 'none';
          const searchIcon = document.querySelector<HTMLElement>('#search-btn');
          if (searchIcon) {
            searchIcon.innerHTML = `<span class="material-symbols-outlined">search</span>`;
          }
        }
      });
    }
  }
}
