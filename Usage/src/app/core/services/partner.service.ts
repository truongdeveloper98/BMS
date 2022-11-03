import { DatePipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { PaginationService } from 'src/app/shared/pagination.service';
import { environment } from 'src/environments/environment';
import { PartnerModel } from '../model/partner.model';
import {
  ProjectViewModel,
  Project,
  Users,
  ProjectTypes,
} from '../model/project';

@Injectable({
  providedIn: 'root',
})
export class PartnerService {
  constructor(private http: HttpClient, private datePipe: DatePipe) {}
  private apiURL = environment.apiUrl + '/partners';

  getAll(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<PartnerModel[]>(this.apiURL + '/GetAll', {
      observe: 'response',
      params,
    });
  }

  getPartnerForProject(type: number): Observable<PartnerModel[]> {
    return this.http.get<PartnerModel[]>(`${this.apiURL}/GetPartner/${type}`);
  }

  changeStatus(id: number, status: boolean) {
    return this.http.patch(`${this.apiURL}/ChangeStatus/${id}`, status);
  }

  edit(model: any) {
    //const formData = this.buildFormData(genre);
    return this.http.put(`${this.apiURL}/UpdatePartner`, model);
  }

  getById(id: number): Observable<PartnerModel> {
    return this.http.get<PartnerModel>(`${this.apiURL}/Info/${id}`);
  }

  CreatePartner(model: any) {
    return this.http.post(this.apiURL + '/CreatePartner', model);
  }
}
