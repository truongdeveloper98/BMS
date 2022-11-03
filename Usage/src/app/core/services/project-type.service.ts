import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ProjectTypes } from '../model/project';

@Injectable({
  providedIn: 'root'
})
export class ProjectTypeService {

  private baseApiUrl = environment.apiUrl;
  constructor(private httpClient: HttpClient) {}

  getProjectTypes() : Observable<ProjectTypes[]>{
    return this.httpClient.get<ProjectTypes[]>(this.baseApiUrl + '/Projects/GetProjectTypes');
  }
}
