import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { FormControl } from '@angular/forms';
import { addDays, addMonths, addYears, format } from 'date-fns';
import { LoadingService } from '../../../../shared/services/loading.service';

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'visitors-chart',
  templateUrl: 'visitors-chart.component.html',
  styleUrl: 'visitors-chart.component.scss',
})
export class VisitorsChartComponent {
  public chartType: IDropdown[] = [
    {
      id: 1,
      name: 'Weekly',
    },
    {
      id: 2,
      name: 'Monthly',
    },
    {
      id: 3,
      name: 'Yearly',
    },
  ];
  public isMenuVisible: boolean = false;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly loadingService: LoadingService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.visitorsChartType.valueChanges.subscribe(() => this.resetDateRange());
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  visitorsChartType = new FormControl(1);
  currentRangeStart: Date = new Date();
  currentRangeEnd: Date = addDays(this.currentRangeStart, 7);

  public resetDateRange(): void {
    console.log(this.visitorsChartType.value);

    const selectedType = this.visitorsChartType.value;

    if (selectedType === 1) {
      // Weekly

      this.currentRangeStart = new Date();
      this.currentRangeEnd = addDays(this.currentRangeStart, 7);
    } else if (selectedType === 2) {
      // Monthly
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addMonths(this.currentRangeStart, 1);

      console.log(this.currentRangeStart, this.currentRangeEnd);
    } else if (selectedType === 3) {
      // Yearly
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addYears(this.currentRangeStart, 1);
    }
  }
  public updateDateRange(direction: 'forward' | 'backward'): void {
    const selectedType = this.visitorsChartType.value;

    let adjustmentValue = 0;
    if (selectedType === 1) adjustmentValue = 7; // Weekly
    else if (selectedType === 2) adjustmentValue = 1; // Monthly
    else if (selectedType === 3) adjustmentValue = 1; // Yearly

    if (direction === 'backward') adjustmentValue *= -1;

    if (selectedType === 1) {
      // Weekly
      this.currentRangeStart = addDays(this.currentRangeStart, adjustmentValue);
      this.currentRangeEnd = addDays(this.currentRangeEnd, adjustmentValue);
    } else if (selectedType === 2) {
      // Monthly
      this.currentRangeStart = addMonths(
        this.currentRangeStart,
        adjustmentValue
      );
    } else if (selectedType === 3) {
      // Yearly
      this.currentRangeStart = addYears(
        this.currentRangeStart,
        adjustmentValue
      );
    }
  }

  public getFormattedRange(): string {
    let date;
    const selectedType = this.visitorsChartType.value;

    if (selectedType === 1) {
      date = `${format(this.currentRangeStart, 'MMM dd yyyy')} - ${format(
        this.currentRangeEnd,
        'MMM dd yyyy'
      )}`;
    } else if (selectedType === 2) {
      date = `${format(this.currentRangeStart, 'MMM yyyy')}`;
    } else if (selectedType === 3) {
      date = `${format(this.currentRangeStart, 'yyyy')}`;
    }
    return date;
  }
}
