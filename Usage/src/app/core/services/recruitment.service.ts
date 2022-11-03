import { CdkNoDataRow } from '@angular/cdk/table';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RecruitmentViewModel } from '../model/recruitment.model';
import {
  ChangePwViewModel,
  changeUserStatus,
  ProfileViewModel,
  RoleViewModel,
  UserTypeViewModel,
  userCreationDTO,
  userDTO,
} from '../model/userService.model';

@Injectable({
  providedIn: 'root',
})
export class ReCruitmentService {
  constructor(private http: HttpClient, private datePipe: DatePipe) {}
  private apiURL = environment.apiUrl + '/recruitments';

  getAll(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<RecruitmentViewModel[]>(this.apiURL + '/GetAll', {
      observe: 'response',
      params,
    });
  }
  edit(model: any) {
    return this.http.put(`${this.apiURL}/UpdateReCruitment`, model);
  }
  CreateReCruitment(model: any) {
    return this.http.post(this.apiURL + '/CreateRecuitment', model);
  }
  getById(id: number): Observable<RecruitmentViewModel> {
    return this.http.get<RecruitmentViewModel>(`${this.apiURL}/GetById/${id}`);
  }

  changeStatus(projectId: number, status: number): Observable<any> {
    status === 1 ? (status = 0) : (status = 1);

    return this.http.patch<any>(
      this.apiURL + '/ChangeStatus/' + projectId,
      status
    );
  }
  changeStatus_Re(projectId: number, status: number): Observable<any> {
    return this.http.patch<any>(
      this.apiURL + '/ChangeResult/' + projectId,
      status
    );
  }

  changeStatusPro(projectId: number, status: number): Observable<any> {
    return this.http.patch<any>(
      this.apiURL + '/ChangePriority/' + projectId,
      status
    );
  }

  getDashboardStat(dateRef: any): Observable<any> {
    return this.http.get(this.apiURL + '/DashboardStat/' + dateRef);
  }

  getDashboardStatpos(dateRef: any): Observable<any> {
    return this.http.get(this.apiURL + '/DashboardStatPosition/' + dateRef);
  }
}
