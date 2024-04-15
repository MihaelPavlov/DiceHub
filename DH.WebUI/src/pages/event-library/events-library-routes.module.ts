import { RouterModule, Routes } from "@angular/router";
import { EventsLibraryComponent } from "./page/events-library.component";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
      path: 'library',
      component: EventsLibraryComponent,
    },
    // {
    //   path: ':id/details',
    //   component: GameDetailsComponent,
    // },
    // {
    //   path: ':id/availability',
    //   component: GameAvailabilityComponent,
    // },
    // {
    //   path: ':id/reviews',
    //   component: GameReviewsComponent,
    // },
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class EventsLibraryRoutingModule {}
  