import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';
import { IMenuItemInterface } from '../models/menu-item.interface';
import { NavigationEnd, Router } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';

@Component({
  selector: 'app-menu',
  templateUrl: 'menu.component.html',
  styleUrl: 'menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MenuComponent implements OnInit, AfterViewInit {
  public leftMenuItems: IMenuItemInterface[] = [];
  public rightMenuItems: IMenuItemInterface[] = [];
  public menuItemWithForceActiveExists: boolean = false;
  private destroy$: Subject<boolean> = new Subject<boolean>();
  public activeLink = NAV_ITEM_LABELS.GAMES;

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly cd: ChangeDetectorRef
  ) {
    this.updateMenuItems();
  }

  public ngOnInit(): void {
    this.menuTabsService.activeTab$
      .pipe(takeUntil(this.destroy$))
      .subscribe((label) => {
        this.setActiveTab(label);
        this.cd.detectChanges();
      });
    this.router.events
      .pipe(
        takeUntil(this.destroy$),
        filter((event) => event instanceof NavigationEnd)
      )
      .subscribe((navEvent: any) => {
        this.activeLink = (navEvent as NavigationEnd).url.split('/')[1];
      });
  }

  public ngAfterViewInit(): void {
    this.onInitJS();
  }

  public ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  public setActiveTab(label: string) {
    let menuItem = this.leftMenuItems
      .concat(this.rightMenuItems)
      .find((item) => item.label === label);

    if (menuItem) {
      menuItem.forceActive = true;
      this.menuItemWithForceActiveExists = true;
    } else {
      this.leftMenuItems.forEach((item) => (item.forceActive = false));
      this.rightMenuItems.forEach((item) => (item.forceActive = false));

      this.menuItemWithForceActiveExists = false;
    }
  }

  public updateMenuItemsWihtPage(page: string) {
    this.leftMenuItems = [
      {
        label: NAV_ITEM_LABELS.GAMES,
        class: page === '/games/library' ? 'active' : '',
        enabled: true,
        visible: true,
        icon: 'kid_star',
        route: '/games/library',
      },
      {
        label: NAV_ITEM_LABELS.MEEPLE,
        class: page === '/meeples/find' ? 'active' : '',
        enabled: true,
        visible: true,
        icon: 'group',
        route: '/meeples/find',
      },
    ];

    this.rightMenuItems = [
      {
        label: NAV_ITEM_LABELS.EVENTS,
        class: page === '/events/library' ? 'active' : '',
        enabled: true,
        visible: true,
        icon: 'stadium',
        route: '/events/library',
      },
      {
        label: NAV_ITEM_LABELS.PROFILE,
        class: page === '/meeple' ? 'active' : '',
        enabled: true,
        visible: true,
        icon: 'account_circle',
        route: '/',
      },
    ];
  }

  private updateMenuItems() {
    let page: string = location.pathname.split('/')[1];
    this.updateMenuItemsWihtPage(page);
  }

  private onInitJS(): void {
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
  }
}
