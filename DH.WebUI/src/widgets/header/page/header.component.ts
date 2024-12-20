import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BehaviorSubject, debounceTime, distinctUntilChanged } from 'rxjs';
import { SearchService } from '../../../shared/services/search.service';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  styleUrl: 'header.component.scss',
})
export class HeaderComponent implements OnInit, AfterViewInit {
  @Input() header!: string;
  @Input() withBackBtn: boolean = false;
  @Input() withHistoryBtn: boolean = false;
  @Input() withSearch: boolean = false;
  @Input() withAdd: boolean = false;
  @Input() withMenu: boolean = false;
  @Input() withQRcode: boolean = false;
  @Input() withBottomLine: boolean = false;
  @Input() menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  @Input() menuItemClickFunction!: (option: string) => void;
  @Output() addClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() backClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() historyClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() searchExpressionResult: EventEmitter<string> =
    new EventEmitter<string>();
  @Output() headerClicked: EventEmitter<void> = new EventEmitter<void>();

  public searchForm!: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly searchService: SearchService,
    private readonly router: Router
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

  public onAddClick(): void {
    this.addClicked.emit();
  }

  public onBackClick(): void {
    this.backClicked.emit();
  }

  public onHistoryClick(): void {
    console.log('history header');
    
    this.historyClicked.emit();
  }

  public onHeaderClick(): void {
    this.headerClicked.emit();
  }

  public onQrCodeClick(): void {
    this.router.navigateByUrl('/qr-code-scanner');
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

      if (withClean && this.searchForm) {
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
    let navbar = document.getElementById('navbar');
    if (!navbar) {
      navbar = document.getElementById('sticky_navbar');
    }
    let prevScrollPos = window.scrollY;
    if (navbar) {
      window.onscroll = () => {
        if (this.getDisabledScrollEvent()) return;

        const currentScrollPos = window.scrollY;
        if (prevScrollPos > currentScrollPos) {
          navbar?.classList.remove('hidden');
        } else {
          navbar?.classList.add('hidden');
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
}
