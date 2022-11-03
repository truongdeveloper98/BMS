import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { HeaderComponent } from '../shared/layout/header/header.component';
import { MenuLeftComponent } from '../shared/layout/menu-left/menu-left.component';
import { ListUsersComponent } from '../users/list-users/list-users.component';
import { MaterialModule } from '../material/material.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { FooterComponent } from '../shared/layout/footer/footer.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RegisterUsersComponent } from '../users/register-users/register-users.component';
import { EditUsersComponent } from '../users/edit-users/edit-users.component';
import { FormUsersComponent } from '../users/form-users/form-users.component';
import { ProjectsComponent } from '../projects/projects.component';
import { ProjectListComponent } from '../projects/project-list/project-list.component';
import { ProjectComponent } from '../projects/project/project.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { ForbiddenComponent } from '../shared/forbidden/forbidden.component';
import { HomeComponent } from '../home/home.component';
import { AccountComponent } from '../account/account.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ProfileComponent } from '../account/profile/profile.component';
import { ChangePasswordComponent } from '../account/change-password/change-password.component';
import { CreateOffReportComponent } from '../report/create-report/create-off-report/create-off-report.component';
import { FormCreateReportComponent } from '../report/create-report/form-create-report/form-create-report.component';
import { ListReportGennericComponent } from '../report/list-report/list-report-genneric/list-report-genneric.component';
import { ListReportOffComponent } from '../report/list-report/list-report-off/list-report-off.component';
import { ManagerReportComponent } from '../report/list-report/manager-report/manager-report.component';
import { MyReportComponent } from '../report/list-report/my-report/my-report.component';
import { TodayReportComponent } from '../report/list-report/today-report/today-report.component';
import { CreateReportComponent } from '../report/create-report/create-report/create-report.component';
import { FormSalaryComponent } from '../setting/salary/form-salary/form-salary.component';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { CdkColumnDef, CdkTableModule } from '@angular/cdk/table';
import { ChartsModule } from 'ng2-charts';
import { ListSalariesComponent } from '../setting/salary/list-salaries/list-salaries.component';
import { ListPartnerComponent } from '../partner/list-partner/list-partner.component';
import { ListPartnerCusComponent } from '../partner/list-partner-cus/list-partner-cus.component';
import { CreatePartnerComponent } from '../partner/create-partner/create-partner.component';
import { FormPartnerComponent } from '../partner/form-partner/form-partner.component';
import { EditPartnerComponent } from '../partner/edit-partner/edit-partner.component';
import { UseronboardComponent } from '../usersonboard/useronboard/useronboard.component';
import { UseronboardListComponent } from '../usersonboard/useronboard-list/useronboard-list.component';
import { FormRecruitmentComponent } from '../recruitment/form-recruitment/form-recruitment.component';
import { CreateRecruitmentComponent } from '../recruitment/create-recruitment/create-recruitment.component';
import { EditRecruitmentComponent } from '../recruitment/edit-recruitment/edit-recruitment.component';
import { ListRecruitmentComponent } from '../recruitment/list-recruitment/list-recruitment.component';
import { RatingComponent } from '../partner/rating/rating.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { CreateFrameworkComponent } from '../setting/create-framework/create-framework.component';


export const DateFormats = {
  parse: {
    dateInput: ['DD/MM/YYYY'],
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@NgModule({
  declarations: [
    RatingComponent,
    CreateFrameworkComponent,
    ListRecruitmentComponent,
    FormRecruitmentComponent,
    CreateRecruitmentComponent,
    EditRecruitmentComponent,
    AdminComponent,
    UserProfileComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    MenuLeftComponent,
    ListUsersComponent,
    HeaderComponent,
    FooterComponent,
    RegisterUsersComponent,
    EditUsersComponent,
    FormUsersComponent,
    ProjectsComponent,
    ListPartnerComponent,
    CreatePartnerComponent,
    FormPartnerComponent,
    EditPartnerComponent,
    ListPartnerCusComponent,
    ProjectListComponent,
    ProjectComponent,
    ForbiddenComponent,
    HomeComponent,
    DashboardComponent,
    AccountComponent,
    ProfileComponent,
    ChangePasswordComponent,
    FormCreateReportComponent,
    CreateReportComponent,
    CreateOffReportComponent,
    ListReportGennericComponent,
    ListReportOffComponent,
    MyReportComponent,
    TodayReportComponent,
    ManagerReportComponent,
    FormSalaryComponent,
    ListSalariesComponent,
    UseronboardComponent,
    UseronboardListComponent,
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    MaterialModule,
    SweetAlert2Module.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    CdkTableModule,
    ChartsModule,
    NgxMatSelectSearchModule,
  ],
  providers: [
    CdkColumnDef,
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE],
    },
    { provide: MAT_DATE_FORMATS, useValue: DateFormats },
  ],
})
export class AdminModule {}
