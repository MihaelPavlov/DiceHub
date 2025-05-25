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
import { IGameReservationStatus } from '../../../entities/games/models/game-reservation-status.model';
import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

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
  @Input() gameReservationStatus: IGameReservationStatus | null = null;
  @Input() menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  @Input() withScrollAnimation: boolean = true;
  @Input() menuItemClickFunction!: (option: string) => void;
  @Output() addClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() backClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() historyClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() searchExpressionResult: EventEmitter<string> =
    new EventEmitter<string>();
  @Output() headerClicked: EventEmitter<void> = new EventEmitter<void>();
  @Output() reservationWarningClicked: EventEmitter<void> =
    new EventEmitter<void>();

  public searchForm!: FormGroup;

  public readonly ReservationStatus = ReservationStatus;

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
  public hasAdditionalActions(): boolean {
    return (
      this.withSearch ||
      this.withAdd ||
      this.withMenu ||
      this.withQRcode ||
      this.withHistoryBtn
    );
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

  public onReservationWarning(): void {
    this.reservationWarningClicked.emit();
  }

  private closeSearch(withClean: boolean = false): void {
    const searchForm = document.getElementById('searchForm') as HTMLElement;
    if (searchForm) {
      searchForm.style.display = 'none';
      const searchIcon = document.querySelector<SVGElement>('#search-btn');

      if (searchIcon) {
        const pathElement = searchIcon.querySelector('path'); // Select the path inside the SVG
        if (pathElement) {
          // Change to search icon
          pathElement.setAttribute(
            'd',
            'M788.38-127.85 535.92-380.31q-30 24.54-73.5 38.04t-83.88 13.5q-106.1 0-179.67-73.53-73.56-73.53-73.56-179.57 0-106.05 73.53-179.71 73.53-73.65 179.57-73.65 106.05 0 179.71 73.56Q631.77-688.1 631.77-582q0 42.69-13.27 83.69t-37.27 70.69l253.46 253.47-46.31 46.3ZM378.54-394.77q79.61 0 133.42-53.81 53.81-53.8 53.81-133.42 0-79.62-53.81-133.42-53.81-53.81-133.42-53.81-79.62 0-133.42 53.81-53.81 53.8-53.81 133.42 0 79.62 53.81 133.42 53.8 53.81 133.42 53.81Z'
          );
        }
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
    if (navbar && this.withScrollAnimation) {
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

          const searchIcon = document.querySelector<SVGElement>('#search-btn');
          if (searchIcon) {
            const pathElement = searchIcon.querySelector('path'); // Select the path inside the SVG
            if (pathElement) {
              // Change to x icon
              pathElement.setAttribute(
                'd',
                'M252-203.69 205.69-252l227-228-227-230L252-758.31l229 230 227-230L754.31-710l-227 230 227 228L708-203.69l-227-230-229 230Z'
              );
            }
          }
        } else {
          closeFunction(false);
          this.searchExpressionResult.emit('');
          const searchInput = document.getElementById(
            'search'
          ) as HTMLInputElement;
          searchInput.value = '';
          this.setDisabledScrollEvent(false);
        }
      });
    }
  }
}
