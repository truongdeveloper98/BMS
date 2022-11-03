import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountComponent } from '../account/account.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { HomeComponent } from '../home/home.component';
import { ProjectsComponent } from '../projects/projects.component';
import { ManagerReportComponent } from '../report/list-report/manager-report/manager-report.component';
import { MyReportComponent } from '../report/list-report/my-report/my-report.component';
import { TodayReportComponent } from '../report/list-report/today-report/today-report.component';
import { ForbiddenComponent } from '../shared/forbidden/forbidden.component';
import { EditUsersComponent } from '../users/edit-users/edit-users.component';
import { ListUsersComponent } from '../users/list-users/list-users.component';
import { RegisterUsersComponent } from '../users/register-users/register-users.component';
import { AdminComponent } from './admin.component';
import { ListSalariesComponent } from '../setting/salary/list-salaries/list-salaries.component';
import { ListPartnerComponent } from '../partner/list-partner/list-partner.component';
import { UseronboardListComponent } from '../usersonboard/useronboard-list/useronboard-list.component';
import { ListRecruitmentComponent } from '../recruitment/list-recruitment/list-recruitment.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    children: [
      { path: 'usersonboard', component: UseronboardListComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'salaries', component: ListSalariesComponent },
      { path: 'users', component: ListUsersComponent },
      { path: 'users/create', component: RegisterUsersComponent },
      { path: 'users/edit/:id', component: EditUsersComponent },
      { path: 'projects', component: ProjectsComponent },
      { path: 'account-settings', component: AccountComponent },
      { path: 'partners', component: ListPartnerComponent },
      { path: 'recruitments', component: ListRecruitmentComponent },
      { path: 'reports', component: MyReportComponent },
      { path: 'today-reports', component: TodayReportComponent },
      { path: 'manager-reports', component: ManagerReportComponent },
      { path: 'forbidden', component: ForbiddenComponent },
      { path: '', redirectTo: '/admin/dashboard', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
