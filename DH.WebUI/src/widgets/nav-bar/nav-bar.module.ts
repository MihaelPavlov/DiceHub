import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { NavBarComponent } from "./page/nav-bar.component";

@NgModule({
    declarations: [NavBarComponent],
    imports: [SharedModule],
    exports: [NavBarComponent],
    providers: [],
  })
  export class NavBarModule {}
  