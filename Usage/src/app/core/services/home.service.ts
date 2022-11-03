import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DocumentViewModel } from '../model/data';
import { UserOnboardVM } from '../model/useronboard';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private httpClient: HttpClient) { }
  private baseApiUrl = environment.apiUrl;

  getClaims(){
    return this.httpClient.get(environment.apiUrl + '/Users/Privacy');
  }

  getUser(){
    return this.httpClient.get(environment.apiUrl + '/Users');
  }

  getDocumentLink(): Observable<DocumentViewModel[]> {
    return this.httpClient.get<DocumentViewModel[]>(this.baseApiUrl + '/BMS/GetDocumentLink');
  }

  setDocumentLink(request: DocumentViewModel){
    return this.httpClient.put<boolean>(this.baseApiUrl + '/BMS/SetDocumentLink', request);
  }

  getPendingMemberList(time: string): Observable<any> {
    return this.httpClient.get(this.baseApiUrl + '/BMS/PendingMemberList/' + time);
  }
  
  getWorkingHourStat(time: string): Observable<any>{
    return this.httpClient.get(this.baseApiUrl + '/BMS/WorkingHourStat/' + time);
  }

  getMembersOnboard(time:string):Observable<any>{
    return this.httpClient.get<any>(this.baseApiUrl + '/BMS/GetMembersOnboard/' + time);
  }
}
