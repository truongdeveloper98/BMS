import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { SocialAuthService } from 'angularx-social-login';
import { Observable, Subscription } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { UsersService } from 'src/app/core/services/user.service';
import * as signalR from '@microsoft/signalr';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { environment } from 'src/environments/environment';
import { SignlrServiceService } from 'src/app/core/services/signlr-service.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  public isExternalAuth!: boolean;
  siderbarStatus: boolean = false;
  @Output() toggleSidebarForMe: EventEmitter<any> = new EventEmitter();
  @Input() events!: Observable<void>;
  private eventsSubscription!: Subscription;

  roleName = '';
  Avatar = '';
  DisplayName = '';
  userID = '';
  email = '';

  constructor(
    public dialog: MatDialog,
    private router: Router,
    private _socialAuthService: SocialAuthService,
    private _authService: AuthenticationService,
    public signalrService: SignlrServiceService,
    private userService: UsersService
  ) {}

  ngOnInit(): void {
    this.eventsSubscription = this.events.subscribe(() => {
      this.toggleSidebarOverlay();
    });

    this._socialAuthService.authState.subscribe((user) => {
      this.isExternalAuth = user != null;
    });

    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    this.roleName =
      decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    if (decodedToken['Avatar'].length > 0) {
      this.Avatar = decodedToken['Avatar'];
    } else {
      this.Avatar = '/assets/img/users.png';
    }

    this.DisplayName = decodedToken['DisplayName'];
    this.userID = decodedToken['UserID'];

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrlRoot + '/toastr', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();
    connection
      .start()
      .then(() => {
        connection.invoke('CheckLogout', this.userID);
      })
      .catch((err) => console.log('Error while starting connection: ' + err));

    connection.on('SendLogoutStatus', (status) => {
      if (status) {
        this.checkUpdateRole();
      }
    });

    connection.on('BroadcastMessage', (email) => {
      this.email = email;
      if (this.userID === email) {
        this.checkUpdateRole();
      }
    });
  }

  checkUpdateRole() {
    this.onLogout();
    this.dialog.open(ConfirmDialogComponent, {
      disableClose: true,
      width: '450px',
      minHeight: '150px',
      data: {
        title: 'Alert',
        message: 'Vui lòng đăng nhập lại',
        labelOK: 'OK',
        labelCancel: 'Cancel',
        //secondDialog: true,
      },
    });
  }

  ngOnDestroy() {
    this.eventsSubscription.unsubscribe();
  }

  onLogout() {
    localStorage.removeItem('token');
    if (this.isExternalAuth) {
      this._authService.signOutExternal();
    }

    this.router.navigate(['/login']);
  }

  toggleSidebar() {
    this.siderbarStatus = !this.siderbarStatus;
    this.toggleSidebarForMe.emit(this.siderbarStatus);
  }

  toggleSidebarOverlay() {
    this.siderbarStatus = !this.siderbarStatus;
    this.toggleSidebarForMe.emit(this.siderbarStatus);
  }
}
