import { NavigationService } from './../../../../shared/services/navigation-service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { InstructionSection } from '../../../../entities/instruction-management/models/instruction.model';
import { INSTRUCTION_LINK_MAPPINGS } from '../../../../entities/instruction-management/constants/instruction.constant';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-instruction-links',
  templateUrl: 'instruction-links.component.html',
  styleUrl: 'instruction-links.component.scss',
})
export class InstructionLinksComponent implements OnInit {
  public currentSection!: InstructionSection;

  constructor(
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly navigationService: NavigationService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    // Listen for changes in the current route and update the links dynamically
    this.activatedRoute.url.subscribe((urlSegments) => {
      const currentPath = urlSegments[0]?.path; // Extract the first path segment

      if (currentPath && INSTRUCTION_LINK_MAPPINGS[currentPath]) {
        this.currentSection = INSTRUCTION_LINK_MAPPINGS[currentPath];
      }
    });
  }

  public navigateBack(): void {
    this.router.navigateByUrl('instructions');
  }

  public navigateToLink(path: string) {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigate([path]);
  }

  public translate(key: string): string {
    return this.translateService.instant(key);
  }
}
