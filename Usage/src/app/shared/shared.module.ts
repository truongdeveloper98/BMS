import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedRoutingModule } from './shared-routing.module';
import { DialogBaseComponent } from './layout/dialog-base/dialog-base.component';
import { FormBaseComponent } from './layout/form-base/form-base.component';
import { NotFoundComponent } from './layout/not-found/not-found.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { SnackBarComponent } from './layout/snack-bar/snack-bar.component';


@NgModule({
  declarations: [
    DialogBaseComponent,
    FormBaseComponent,
    NotFoundComponent,
    ForbiddenComponent,
    SnackBarComponent
  ],
  imports: [
    CommonModule,
    SharedRoutingModule
  ]
})
export class SharedModule { }
