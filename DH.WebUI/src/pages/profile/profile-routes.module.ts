import { RouterModule, Routes } from "@angular/router";
import { ProfileComponent } from "./page/profile.component";
import { NgModule } from "@angular/core";
import { GlobalSettingsComponent } from "../../features/profile/components/global-settings/global-settings.component";

const routes: Routes = [
    {
      path: '',
      component: ProfileComponent,
    },
    {
        path: 'settings',
        component: GlobalSettingsComponent,
      }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class ProfileRoutingModule {}