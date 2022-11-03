import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  FrameworkVM,
  LanguageVM,
  LevelVM,
  PositionVM,
} from '../model/setting.model';
@Injectable({
  providedIn: 'root',
})
export class SettingService {
  private baseApiUrl = environment.apiUrl;
  constructor(private httpClient: HttpClient) {}

  getPositions(): Observable<PositionVM[]> {
    return this.httpClient.get<PositionVM[]>(
      this.baseApiUrl + '/Setting/GetPositions'
    );
  }

  getLevels(): Observable<LevelVM[]> {
    return this.httpClient.get<LevelVM[]>(
      this.baseApiUrl + '/Setting/GetLevels'
    );
  }

  createOther(name: string, type: number) {
    if (type === 1) {
      return this.httpClient.post(
        this.baseApiUrl + '/Setting/CreateFramework',
        name
      );
    } else {
      return this.httpClient.post(
        this.baseApiUrl + '/Setting/CreatePosition',
        name
      );
    }
  }

  getLanguages(): Observable<LanguageVM[]> {
    return this.httpClient.get<LanguageVM[]>(
      this.baseApiUrl + '/Setting/GetLanguages'
    );
  }

  getFrameworks(): Observable<FrameworkVM[]> {
    return this.httpClient.get<FrameworkVM[]>(
      this.baseApiUrl + '/Setting/GetFrameworks'
    );
  }
}
