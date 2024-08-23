import {
  AfterViewInit,
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { SearchService } from '../../../shared/services/search.service';
import { IMenuItem } from '../../../shared/models/menu-item.model';

@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  styleUrl: 'header.component.scss',
})
export class HeaderComponent implements OnInit, AfterViewInit {
  @Input() header!: string;
  @Input() withSearch: boolean = false;
  @Input() withAdd: boolean = false;
  @Input() withQRcode: boolean = false;
  @Input() withBottomLine: boolean = false;
  @Input() menuItems: IMenuItem[] = [];
  @Input() menuItemClickFunction!: (option: string) => void;
  @Output() addClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() searchExpressionResult: EventEmitter<string> =
    new EventEmitter<string>();

  public searchForm!: FormGroup;
  public isMenuVisible: boolean = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly searchService: SearchService
  ) {
    this.searchService.searchFormVisible$.subscribe((x) => {
      if (x === false) {
        this.closeSearch(true);
        this.setDisabledScrollEvent(false);
      }
    });
  }

  public onMenuItemClick(key: string) {
    if (this.menuItemClickFunction) {
      this.menuItemClickFunction(key);
    }
    this.isMenuVisible = false;
  }

  public ngOnInit(): void {
    this.searchForm = this.fb.group({
      search: [''],
    });

    this.searchForm
      .get('search')!
      .valueChanges.pipe(debounceTime(1000), distinctUntilChanged())
      .subscribe((searchExpression: string) => {
        this.onSearchSubmit(searchExpression);
      });
  }

  public ngAfterViewInit(): void {
    this.initSearchListenersJS();
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  private onSearchSubmit(searchExpression: string) {
    this.searchExpressionResult.emit(searchExpression);
  }

  private closeSearch(withClean: boolean = false): void {
    const searchForm = document.getElementById('searchForm') as HTMLElement;
    if (searchForm) {
      searchForm.style.display = 'none';
      const searchIcon = document.querySelector<HTMLElement>('#search-btn');
      if (searchIcon) {
        searchIcon.innerHTML = `<span class="material-symbols-outlined">search</span>`;
      }

      if (withClean) {
        this.searchForm.get('search')?.reset();
      }
    }
  }
  private disabledScrollEvent = false;
  private setDisabledScrollEvent(value: boolean) {
    this.disabledScrollEvent = value;
  }

  private getDisabledScrollEvent(): boolean {
    return this.disabledScrollEvent;
  }
  private initSearchListenersJS(): void {
    const navbar = document.getElementById('navbar');
    let prevScrollPos = window.scrollY;
    if (navbar) {
      window.onscroll = () => {
        if (this.getDisabledScrollEvent()) return;

        const currentScrollPos = window.scrollY;
        if (prevScrollPos > currentScrollPos) {
          navbar.classList.remove('hidden');
        } else {
          navbar.classList.add('hidden');
        }
        prevScrollPos = currentScrollPos;
      };
    }

    const searchForm = document.getElementById('searchForm') as HTMLElement;
    const closeFunction = this.closeSearch;
    if (searchForm) {
      searchForm.style.display = 'none';
      this.setDisabledScrollEvent(false);
    }

    const wrapperSearch =
      document.querySelector<HTMLElement>('.wrapper_search');
    if (wrapperSearch) {
      wrapperSearch.addEventListener('click', () => {
        if (searchForm.style.display === 'none') {
          this.setDisabledScrollEvent(true);
          searchForm.style.display = 'block';

          const searchIcon = document.querySelector<HTMLElement>('#search-btn');
          if (searchIcon) {
            searchIcon.innerHTML = `<span class="material-symbols-outlined">close</span>`;
          }
        } else {
          closeFunction(false);
          this.setDisabledScrollEvent(false);
        }
      });
    }
  }

  @HostListener('window:scroll', [])
  private onWindowScroll(): void {
    if (this.isMenuVisible) {
      this.isMenuVisible = false;
    }
  }
}
