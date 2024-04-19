import { RouterModule, Routes } from "@angular/router";
import { EventsLibraryComponent } from "./page/events-library.component";
import { NgModule } from "@angular/core";
import { EventDetailsComponent } from "../../features/events-library/components/event-details/page/event-details.component";

const routes: Routes = [
    {
      path: 'library',
      component: EventsLibraryComponent,
    },
    {
      path: ':id/details',
      component: EventDetailsComponent,
    }
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class EventsLibraryRoutingModule {}
  