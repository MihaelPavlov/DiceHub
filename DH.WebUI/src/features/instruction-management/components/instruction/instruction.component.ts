import { NavigationService } from './../../../../shared/services/navigation-service';
import { Component, OnInit } from '@angular/core';
import {
  InstructionSection ,
  InstructionTopic,
} from '../../../../entities/instruction-management/models/instruction.model';
import { ActivatedRoute, Router } from '@angular/router';
import { INSTRUCTION_LINK_MAPPINGS } from '../../../../entities/instruction-management/constants/instruction.constant';

@Component({
  selector: 'app-instruction',
  templateUrl: 'instruction.component.html',
  styleUrl: 'instruction.component.scss',
})
export class InstructionComponent implements OnInit {
  public selectedInstruction!: InstructionSection  | null;
  public selectedTopic!: InstructionTopic | null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly navigationService: NavigationService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const key = params.get('key'); // Get 'key' from the route

      if (key && INSTRUCTION_LINK_MAPPINGS[key]) {
        // Find the specific link inside the instruction
        this.selectedInstruction = INSTRUCTION_LINK_MAPPINGS[key];
        this.selectedTopic =
          this.selectedInstruction.topics.find(
            (link) => this.slugify(link.route) === this.router.url
          ) || null;
      }
    });
  }

  public navigateBack(): void {
    this.router.navigateByUrl(
      this.navigationService.getPreviousUrl() ?? 'instructions'
    );
  }

  private slugify(text: string): string {
    return text.toLowerCase().replace(/\s+/g, '-'); // Convert names to URL-friendly format
  }
}
