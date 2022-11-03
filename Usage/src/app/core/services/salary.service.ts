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
import { Users } from '../model/project';
import { SalaryInfo, SalaryViewModel } from '../model/setting.model';

@Injectable({
  providedIn: 'root',
})
export class SalaryService {  
  form: FormGroup = new FormGroup({
    id: new FormControl(null),
    effectiveDate: new FormControl('', Validators.required),
    hourlySalary: new FormControl(0, Validators.required),
    listMembers: new FormControl([], Validators.required)    
  });

  private baseApiUrl = environment.apiUrl;
  constructor(
    private httpClient: HttpClient,
    private datePipe: DatePipe,
    private paginationService: PaginationService
  ) {}

  selectedMemberIds: string[] = [];

  initializeFormGroup() {
    this.form.setValue({
      id: null,      
      effectiveDate: '',
      hourlySalary: 0,
      listMembers: [],      
    });    
  }

  insertSalary(salary: any): Observable<any> {
    const request: SalaryViewModel = {
      Id: null,
      HourlySalary:salary.hourlySalary,
      EffectiveDate: this.datePipe.transform(salary.effectiveDate, 'yyyy-MM-dd'),
      User_Id: salary.listMembers
    };

    return this.httpClient.post<any>(
      this.baseApiUrl + '/Setting/CreateSalary',
      request
    );
  }

  updateSalary(salary: any): Observable<any> {
    const request: SalaryViewModel = {
      Id: salary.id,
      HourlySalary:salary.hourlySalary,
      EffectiveDate: this.datePipe.transform(salary.effectiveDate, 'yyyy-MM-dd'),
      User_Id: salary.listMembers        
    };

    return this.httpClient.put<any>(
      this.baseApiUrl + '/Setting/UpdateSalary',
      request
    );
  }

  getSalaries(values: any): Observable<any> {

    const params = new HttpParams()
      .set('page', this.paginationService.page)
      .set('recordsPerPage', this.paginationService.pageCount);

    return this.httpClient.get<SalaryInfo[]>(
      this.baseApiUrl + '/Setting/GetSalaries',
      { observe: 'response', params }
    );
  }

  getSalary(id: any) {
    this.httpClient
      .get<SalaryInfo>(this.baseApiUrl + '/Setting/GetSalary/' + id)
      .subscribe(
        (res) => {
          this.form.setValue({
            id: res.Id,
            hourlySalary:res.HourlySalary,
            effectiveDate:res.EffectiveDate,
            listMembers: [],
          });
          this.selectedMemberIds=new Array<string>(res.User_Id);
          this.setDisable();          
        },
        (err) => {
          console.log(err);
        }
      );
  }

  getUsers(): Observable<Users[]> {
    return this.httpClient.get<Users[]>(this.baseApiUrl + '/Users/getAlls');
  }

  changeStatus(salaryId: number, isDeleted: boolean): Observable<any> {
    isDeleted === true ? (isDeleted = false) : (isDeleted = true);

    return this.httpClient.patch<any>(
      this.baseApiUrl + '/Setting/ChangeSalaryStatus/' + salaryId,
      isDeleted
    );
  }

  setDisable(){  
    this.form.get('listMembers')?.disable();
  }
}
