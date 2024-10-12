import { AuthService } from './../../../../entities/auth/auth.service';
import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ISpaceTableList } from '../../../../entities/space-management/models/space-table-list.model';

@Component({
  selector: 'app-club-space-list',
  templateUrl: 'club-space-list.component.html',
  styleUrl: 'club-space-list.component.scss',
})
export class ClubSpaceListComponent {
  public spaceTableList$!: Observable<ISpaceTableList[] | null>;

  constructor(
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService,
  ) {}

  public get getCurrentUserId(): string | undefined {
    return this.authService.getUser?.id;
  }

  public ngOnInit(): void {
    this.spaceTableList$ = this.spaceManagementService.getList();
  }

  public handleSearchExpression(searchExpression: string) {
    this.spaceTableList$ =
      this.spaceManagementService.getList(searchExpression);
  }
}
