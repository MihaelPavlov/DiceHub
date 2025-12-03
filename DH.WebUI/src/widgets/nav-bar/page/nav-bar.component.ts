import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

export interface Column {
  name: string;
  link: string;
  isActive: boolean;
}

@Component({
    selector: 'app-nav-bar',
    templateUrl: 'nav-bar.component.html',
    styleUrl: 'nav-bar.component.scss',
    standalone: false
})
export class NavBarComponent implements OnInit {
  @Input() columns: Column[] = [];

  constructor(private readonly router: Router) {}

  public ngOnInit(): void {
    this.columns.forEach((column) => {
      if (column.isActive) {
        // this.router.navigateByUrl(column.link);
      }
    });
  }

  public toggleActive(item: Column): void {
    // Don't preemptively mark as active
    this.router.navigateByUrl(item.link).then((navigated) => {
      if (navigated) {
        // If navigation succeeded, update active states
        this.columns.forEach((column) => (column.isActive = column === item));
      }
    });
  }
}
