import { CdkNoDataRow } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserForReportVm } from '../model/report.model';
import {
  ChangePwViewModel,
  changeUserStatus,
  ProfileViewModel,
  RoleViewModel,
  UserTypeViewModel,
  userCreationDTO,
  userDTO,
  CompanyViewModel,
} from '../model/userService.model';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private http: HttpClient, private datePipe: DatePipe) {}
  private apiURL = environment.apiUrl + '/users';

  getAll(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<userDTO[]>(this.apiURL + '/FilterUsers', {
      observe: 'response',
      params,
    });
  }
  searchByName(name: string): Observable<UserForReportVm[]> {
    const headers = new HttpHeaders('Content-Type: application/json');
    return this.http.post<UserForReportVm[]>(
      `${this.apiURL}/SearchByName`,
      JSON.stringify(name),
      { headers }
    );
  }
  getUserForReport(): Observable<UserForReportVm[]> {
    return this.http.get<UserForReportVm[]>(this.apiURL + '/GetUserForReport');
  }
  changeStatus(id: string, date: any) {
    return this.http.patch(`${this.apiURL}/UpdateStatus/${id}`, date);
  }
  edit(model: any) {
    return this.http.put(`${this.apiURL}/Update`, model);
  }

  getRole(): Observable<RoleViewModel[]> {
    return this.http.get<RoleViewModel[]>(this.apiURL + '/GetRoles');
  }
  CreateUser(model: any) {

    return this.http.post(environment.apiUrl + '/accounts/Register', model);
  }
  getById(id: string): Observable<userDTO> {
    return this.http.get<userDTO>(`${this.apiURL}/GetUser/${id}`);
  }

  getProfile(): Observable<any>{
    return this.http.get<any>(`${this.apiURL}/GetProfile`);
  }

  uploadImage(id:string,file:File):Observable<any>{
    const formData = new FormData();
    formData.append("profileImage",file);
    return this.http.post(`${this.apiURL}/UpdateAvatar/`+ id,
     formData,{
       responseType:'text'
     });
  }

  updateProfile(user:ProfileViewModel):Observable<any>{

    return this.http.put<any>(`${this.apiURL}/UpdateProfile`,user);
  }

  ChangePassword(request: ChangePwViewModel):Observable<any>{
    return this.http.put<any>(`${this.apiURL}/ChangePassword`,request);
  }

  roleMatch(allowedRoles: string[]): boolean {
    var isMatch = false;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    var userRole =
      decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];

    for (let i = 0; i < allowedRoles.length; i++) {
      if (userRole == allowedRoles[i]) {
        isMatch = true;
        break;
      }
    }
    return isMatch;
  }

  getTypeList(): Observable<UserTypeViewModel[]> {
    return this.http.get<UserTypeViewModel[]>(`${this.apiURL}/GetTypeList`);
  }

  myLeaveDayData(): Observable<any>{
    return this.http.get<any>(`${this.apiURL}/MyLeaveDayData`);
  }

  getMemberStat(dateRef: any): Observable<any>{
    return this.http.get<any>(`${this.apiURL}/MemberStat/` + dateRef);
  }

  getCompanies(): Observable<CompanyViewModel[]> {
    return this.http.get<CompanyViewModel[]>(`${this.apiURL}/Companies`);
  }
}
