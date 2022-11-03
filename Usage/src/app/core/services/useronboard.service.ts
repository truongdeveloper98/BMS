import { DatePipe } from '@angular/common';
import {
  HttpClient,
  HttpParams,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { PaginationService } from 'src/app/shared/pagination.service';
import { environment } from 'src/environments/environment';
import { UserOnboardVM } from '../model/useronboard';

@Injectable({
  providedIn: 'root',
})
export class UserOnboardService {  
  form: FormGroup = new FormGroup({
    id: new FormControl(null),
    fullName: new FormControl('', Validators.required),
    position: new FormControl(null, Validators.required),
    language: new FormControl(''),
    level: new FormControl(null, Validators.required),
    onboardDate: new FormControl('', Validators.required),
    note:    new FormControl('')
  });

  private baseApiUrl = environment.apiUrl;
  constructor(
    private httpClient: HttpClient,
    private datePipe: DatePipe,
    private paginationService: PaginationService
  ) {}

  initializeFormGroup() {
    this.form.setValue({
      id: null,
      fullName:'',
      position:null,
      language:'',
      level:null,
      onboardDate: '',
      note:''     
    });    
  }

  getUsersOnboard(values: any): Observable<any> {
    const params = new HttpParams()
      .set('page', this.paginationService.page)
      .set('recordsPerPage', this.paginationService.pageCount)
      .set('search', values.search ?? '');

    return this.httpClient.get<UserOnboardVM[]>(
      this.baseApiUrl + '/Users/GetUsersOnboard',
      { observe: 'response', params }
    );
  }

  getUserOnboard(id: any) {
    this.httpClient
      .get<UserOnboardVM>(this.baseApiUrl + '/Users/GetUserOnboard/' + id)
      .subscribe(
        (res) => {
          this.form.setValue({
            id: res.Id,
            fullName:res.FullName,
            position:res.Position,
            level: res.Level,
            language: res.Language,
            onboardDate: res.OnboardDate,
            note:res.Note
          });         
        },
        (err) => {
          console.log(err);
        }
      );
  }

  insertUserOnboard(userOnboard: any): Observable<any> {
    const request: UserOnboardVM = {
      Id: null,
      FullName:userOnboard.fullName,
      Position:userOnboard.position,
      Level:userOnboard.level,
      Language:userOnboard.language,
      OnboardDate: this.datePipe.transform(userOnboard.onboardDate, 'yyyy-MM-dd'),
      Note:userOnboard.note
    };

    return this.httpClient.post<any>(
      this.baseApiUrl + '/Users/CreateUserOnboard',
      request
    );
  }

  updateUserOnboard(userOnboard: any): Observable<any> {
    const request: UserOnboardVM = {
      Id: userOnboard.id,
      FullName:userOnboard.fullName,
      Position:userOnboard.position,
      Level:userOnboard.level,
      Language:userOnboard.language,
      OnboardDate: this.datePipe.transform(userOnboard.onboardDate, 'yyyy-MM-dd'),
      Note:userOnboard.note        
    };

    return this.httpClient.put<any>(
      this.baseApiUrl + '/Users/UpdateUserOnboard',
      request
    );
  }


  changeUserOnboardStatus(memberId: number, isDeleted: boolean): Observable<any> {
    isDeleted === true ? (isDeleted = false) : (isDeleted = true);
    return this.httpClient.patch<any>(
      this.baseApiUrl + '/Users/ChangeUserOnboardStatus/' + memberId,
      isDeleted
    );
  }


}
