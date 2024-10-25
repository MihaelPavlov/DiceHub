import { RouterModule, Routes } from "@angular/router";
import { ProfileComponent } from "./page/profile.component";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
      path: '',
      component: ProfileComponent,
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class ProfileRoutingModule {}