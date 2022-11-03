import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SocialAuthService } from 'angularx-social-login';
import { AuthenticationService } from 'src/app/core/services/authentication.service';

@Component({
  selector: 'app-forbidden',
  templateUrl: './forbidden.component.html',
  styleUrls: ['./forbidden.component.scss']
})
export class ForbiddenComponent implements OnInit {
  public isExternalAuth!: boolean;
  private _returnUrl: string | undefined;
  
  constructor( private _router: Router, private _route: ActivatedRoute,private _socialAuthService: SocialAuthService,private _authService: AuthenticationService) { }
  ngOnInit(): void {
    this._returnUrl = this._route.snapshot.queryParams['returnUrl'] || '/';
    this._socialAuthService.authState.subscribe(user => {
      this.isExternalAuth = user != null;
    })
  }
  public navigateToLogin = () => {
    localStorage.removeItem('token');
    if(this.isExternalAuth){
      this._authService.signOutExternal();
    }

    this._router.navigate(['/login'], { queryParams: { returnUrl: this._returnUrl }});
  }

}
