import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { environment } from '../../../environments/environment';
import { SocialUser } from 'angularx-social-login';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  adminLogin: boolean = false;
  public showError: boolean | undefined;
  public showErrorAlert: boolean | undefined;
  public errorMessage: string = '';
  public newUser: boolean = false;

  formModel = {
    Username: '',
    Password: '',
  };

  isLoading: boolean = false;
  loginGoogleLabel: string = 'Login with Google';

  constructor(
    private http: HttpClient,
    private router: Router,
    private _authService: AuthenticationService,
    private toastr: ToastrService,
    private jwtHelper: JwtHelperService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    if (localStorage.getItem('token') != null && !this.jwtHelper.isTokenExpired(localStorage.getItem('token')!)) {
      this.router.navigateByUrl('/admin/dashboard');
    }
  }

  onSubmit(form: NgForm) {
    this.showErrorAlert = false;
    var body = form.value;
    this.http.post(environment.apiUrl + '/Accounts/Login', body).subscribe(
      (res: any) => {
        localStorage.setItem('token', res.token);
        this.router.navigateByUrl('/admin/dashboard');
      },
      (err) => {
        if (err.status == 400) {
          this.showErrorAlert = true;
          this.errorMessage = err.error.message;

        } else {
          this.toastr.error("CONNECTION REFUSED");
        }
      }
    );
  }

  public externalLogin = () => {
    this.isLoading = true;
    this.loginGoogleLabel = 'Logging in...'
    this.showError = false;
    this._authService.signInWithGoogle()
      .then(res => {
        
        const user: SocialUser = { ...res };

        const externalAuth = {
          provider: user.provider,
          idToken: user.idToken
        }
        this.validateExternalAuth(externalAuth);

      }, error => {
        console.log(error)
        if (error.status == 400) {
          this.showErrorAlert = true;
          this.errorMessage = error.error.message;
        } else {
          this.toastr.error("CONNECTION REFUSED");
          this.isLoading = false;
          this.loginGoogleLabel = 'Login with Google';
        }
      })
  }

  private validateExternalAuth(externalAuth: any) {
    this._authService.externalLogin('Accounts/ExternalLogin', externalAuth)
      .subscribe(res => {
        this.newUser = res.IsNewUser;
        localStorage.setItem("token", res.Token);
        this._authService.sendAuthStateChangeNotification(res.IsAuthSuccessful);
        this.router.navigate(["/admin/dashboard"]);
        if (res.IsNewUser) {
          this.router.navigate(["/admin/account-settings"]);
          this.dialog
            .open(ConfirmDialogComponent, {
              disableClose: true,
              width: '450px',
              minHeight: '150px',
              data: {
                title: 'Update profile',
                message: 'Bạn cần đổi mật khẩu và cập nhật thông tin cá nhân. Mật khẩu mặc định: 123456Aa@',
                labelOK: 'Yes',
                labelCancel: 'No',
              },
            })
        }
      },
        error => {
          if (error.status == 400) {
            this.showErrorAlert = true;
            console.log(error);
            this.errorMessage = error.error.message;
          } else {
            this.toastr.error("CONNECTION REFUSED");
          }
          this._authService.signOutExternal();
        });
  }

}
