import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { DatePipe } from '@angular/common';
import {
  SocialLoginModule,
  SocialAuthServiceConfig,
} from 'angularx-social-login';
import { GoogleLoginProvider } from 'angularx-social-login';
import { MaterialModule } from './material/material.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './users/login/login.component';
import { HttpInterceptorService } from './shared/http-interceptor.service';
import { EditUsersComponent } from './users/edit-users/edit-users.component';
import { RegisterUsersComponent } from './users/register-users/register-users.component';
import { CustomPipeModule } from './core/pipe/custom-pipe.module';
import { CustomPreloadingStrategy } from './shared/custom-preloading-strategy';
import { DateVNPipe } from './core/pipe/date-vn.pipe';
import { environment } from 'src/environments/environment';
import { CheckReportComponent } from './report/check-report/check-report.component';
import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';
import { PendingPopupComponent } from './dashboard/pending-popup/pending-popup.component';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    CheckReportComponent,
    ConfirmDialogComponent,
    PendingPopupComponent,
  ],
  imports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    FormsModule,
    MaterialModule,
    CustomPipeModule,
    ReactiveFormsModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [environment.JwtModuleAllowedDomains],
        disallowedRoutes: [],
      },
    }),
    SocialLoginModule,
  ],
  providers: [
    DatePipe,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpInterceptorService,
      multi: true,
    },
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(environment.GoogleLoginProvider),
          },
        ],
      } as SocialAuthServiceConfig,
    },
    CustomPreloadingStrategy,
    DateVNPipe,
  ],
  bootstrap: [AppComponent],
  entryComponents: [EditUsersComponent, RegisterUsersComponent],
})
export class AppModule {}
