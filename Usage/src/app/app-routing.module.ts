import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/auth.guard';
import { LoginComponent } from './users/login/login.component';

import { CustomPreloadingStrategy } from './shared/custom-preloading-strategy';
import { NotFoundComponent } from './shared/layout/not-found/not-found.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: "admin", loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule), data: { preload: true, delay: 5000 }, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: '**', component: NotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { preloadingStrategy: CustomPreloadingStrategy, onSameUrlNavigation: 'reload', })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
