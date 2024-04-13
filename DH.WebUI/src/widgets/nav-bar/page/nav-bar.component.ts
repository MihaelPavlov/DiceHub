import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface Column {
  name: string;
  link: string;
  isActive: boolean;
}

@Component({
  selector: 'app-nav-bar',
  templateUrl: 'nav-bar.component.html',
  styleUrl: 'nav-bar.component.scss',
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
    this.columns.forEach((column) => {
      column.isActive = false;
    });
    item.isActive = true;
    this.router.navigateByUrl(item.link);
  }
}
