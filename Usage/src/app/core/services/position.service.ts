import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Positions } from '../model/project';

@Injectable({
  providedIn: 'root'
})
export class PositionService {
  
  private baseApiUrl = environment.apiUrl;
  constructor(private httpClient: HttpClient) {}

  getPositions() : Observable<Positions[]>{

    return this.httpClient.get<Positions[]>(this.baseApiUrl + '/Projects/GetPositions');
  }
}
