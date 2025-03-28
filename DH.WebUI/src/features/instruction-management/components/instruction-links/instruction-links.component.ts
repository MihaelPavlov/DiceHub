import { NavigationService } from './../../../../shared/services/navigation-service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Instruction } from '../../../../entities/instruction-management/models/instruction.model';
import { INSTRUCTION_LINK_MAPPINGS } from '../../../../entities/instruction-management/constants/instruction.constant';

@Component({
  selector: 'app-instruction-links',
  templateUrl: 'instruction-links.component.html',
  styleUrl: 'instruction-links.component.scss',
})
export class InstructionLinksComponent implements OnInit {
  public currentLink!: Instruction;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private navigationService: NavigationService
  ) {}

  public ngOnInit(): void {
    this.setBackBtnLink();
    // Listen for changes in the current route and update the links dynamically
    this.activatedRoute.url.subscribe((urlSegments) => {
      const currentPath = urlSegments[0]?.path; // Extract the first path segment

      if (currentPath && INSTRUCTION_LINK_MAPPINGS[currentPath]) {
        this.currentLink = INSTRUCTION_LINK_MAPPINGS[currentPath];
      }
    });
  }

  public navigateBack(): void {
    this.router.navigateByUrl(
      this.navigationService.getPreviousUrl() ?? 'instructions'
    );
  }

  public navigateToLink(path: string) {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigate([path]);
  }

  private setBackBtnLink(): void {
    this.navigationService.setPreviousUrl('instructions');
  }
}
