import { Component, OnDestroy, OnInit } from '@angular/core';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { Observable } from 'rxjs';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';

@Component({
  selector: 'app-meeple-room-details',
  templateUrl: 'meeple-room-details.component.html',
  styleUrl: 'meeple-room-details.component.scss',
})
export class MeepleRoomDetailsComponent implements OnInit, OnDestroy {
  public room$!: Observable<IRoomByIdResult>;
  private roomId!: number;

  constructor(
    private readonly roomService: RoomsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = params['id'];      
      this.fetchRoom();
    });
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('meeples/find');
  }

  public navigateToGameDetails(id:number):void{
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public fetchRoom(): void {
    this.room$ = this.roomService.getById(this.roomId);
  }
}
