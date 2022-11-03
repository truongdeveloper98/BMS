import { DatePipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { PaginationService } from 'src/app/shared/pagination.service';
import { environment } from 'src/environments/environment';
import {
  ProjectViewModel,
  Project,
  Users,
  ProjectTypes,
} from '../model/project';
@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  projectTypes!: ProjectTypes[];

  form: FormGroup = new FormGroup({
    id: new FormControl(null),
    partnerId: new FormControl(0),
    customerId: new FormControl(0),
    name: new FormControl('', Validators.required),
    projectCode: new FormControl('', Validators.required),
    revenue: new FormControl(0, Validators.required),
    projectType: new FormControl('0', Validators.required),
    descripion: new FormControl(''),
    backloglink: new FormControl(''),
    startDate: new FormControl('', Validators.required),
    endDate: new FormControl('', Validators.required),
    pmEstimate: new FormControl(0),
    brseEstimate: new FormControl(0),
    comtorEstimate: new FormControl(0),
    testerEstimate: new FormControl(0),
    developerEstimate: new FormControl(0),
    listPMs: new FormControl([]),
    listMembers: new FormControl([]),
  });

  private baseApiUrl = environment.apiUrl;
  constructor(
    private httpClient: HttpClient,
    private datePipe: DatePipe,
    private paginationService: PaginationService
  ) {}

  partnerId!: number;
  customerId!: number;
  selectedPmId: string[] = [];
  selectedMemberId: string[] = [];
  selectedProjectType = '0';
  mindate!: Date;
  maxdate!: Date;
  isPm: number = 0;

  initializeFormGroup() {
    this.form.setValue({
      id: null,
      name: '',
      partnerId: null,
      customerId: null,
      projectCode: '',
      revenue: 0,
      projectType: '0',
      descripion: '',
      backloglink: '',
      startDate: '',
      endDate: '',
      pmEstimate: 0,
      brseEstimate: 0,
      comtorEstimate: 0,
      testerEstimate: 0,
      developerEstimate: 0,
      listPMs: [],
      listMembers: [],
    });
  }
  getMinDate(): Date {
    this.form.get('startDate')?.valueChanges.subscribe((value) => {
      this.mindate = value;
    });
    return this.mindate;
  }
  getMaxDate(): Date {
    this.form.get('endDate')?.valueChanges.subscribe((value) => {
      this.maxdate = value;
    });
    return this.maxdate;
  }
  getProjects(values: any): Observable<any> {
    const params = new HttpParams()
      .set('page', this.paginationService.page)
      .set('recordsPerPage', this.paginationService.pageCount)
      .set('search', values.search ?? '')
      .set('status', values.status ?? '')
      .set('projectType', values.projectType ?? '')
      .set('project', values.project ?? '');

    return this.httpClient.get<Project[]>(
      this.baseApiUrl + '/Projects/GetAll',
      { observe: 'response', params }
    );
  }

  getDetailsProjectPartner(id: any): Observable<ProjectViewModel> {
    return this.httpClient.get<ProjectViewModel>(
      this.baseApiUrl + '/Projects/Info/' + id
    );
  }

  getProject(id: any) {
    this.httpClient
      .get<ProjectViewModel>(this.baseApiUrl + '/Projects/Info/' + id)
      .subscribe(
        (res) => {
          this.form.setValue({
            id: res.Id,
            name: res.Project_Name,
            projectCode: res.Project_Code,
            revenue: res.Revenua,
            projectType: res.ProjectTypeId.toString(),
            descripion: res.Description,
            startDate: res.StartDate,
            endDate: res.EndDate,
            pmEstimate: res.PM_Estimate,
            brseEstimate: res.Brse_Estimate,
            comtorEstimate: res.Comtor_Estimate,
            testerEstimate: res.Tester_Estimate,
            developerEstimate: res.Developer_Estimate,
            listPMs: [],
            listMembers: [],
            partnerId: res.PartnerId,
            customerId: res.CustomerId,
            backloglink: res.BacklogLink,
          });

          this.selectedProjectType = res.ProjectTypeId.toString();

          for (let i = 0; i < res.UserPositions.length; i++) {
            if (res.UserPositions[i].PositionId == 1) {
              this.selectedPmId = res.UserPositions[i].User_Id;
            }
            if (res.UserPositions[i].PositionId == 2) {
              this.selectedMemberId = res.UserPositions[i].User_Id;
            }
          }

          const helper = new JwtHelperService();
          const decodedToken = helper.decodeToken(
            localStorage.getItem('token')!
          );

          var roleName =
            decodedToken[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ];
          this.isPm = +decodedToken['isPm'];
            
          if (roleName !== 'Manager' && roleName !== 'SysAdmin' && this.isPm <= 0) {
            this.setDisable();
          }
        },
        (err) => {
          console.log(err);
        }
      );
  }

  setDisable() {
    this.form.get('name')?.disable();
    this.form.get('partnerId')?.disable();
    this.form.get('customerId')?.disable();
    this.form.get('projectCode')?.disable();
    this.form.get('revenue')?.disable();
    //this.form.get('projectType')?.disable();
    this.form.get('startDate')?.disable();
    this.form.get('endDate')?.disable();
    this.form.get('pmEstimate')?.disable();
    this.form.get('brseEstimate')?.disable();
    this.form.get('comtorEstimate')?.disable();
    this.form.get('testerEstimate')?.disable();
    this.form.get('developerEstimate')?.disable();
    this.form.get('listPMs')?.disable();
    this.form.get('backloglink')?.disable();
  }

  setEnable() {
    this.form.get('name')?.enable();
    this.form.get('partnerId')?.enable();
    this.form.get('customerId')?.enable();
    this.form.get('projectCode')?.enable();
    this.form.get('revenue')?.enable();
    this.form.get('backloglink')?.enable();
    //this.form.get('projectType')?.enable();
    this.form.get('startDate')?.enable();
    this.form.get('endDate')?.enable();
    this.form.get('pmEstimate')?.enable();
    this.form.get('brseEstimate')?.enable();
    this.form.get('comtorEstimate')?.enable();
    this.form.get('testerEstimate')?.enable();
    this.form.get('developerEstimate')?.enable();
    this.form.get('listPMs')?.enable();
  }

  getUsers(): Observable<Users[]> {
    return this.httpClient.get<Users[]>(this.baseApiUrl + '/Users/getAlls');
  }

  getUserPM(): Observable<Users[]> {
    return this.httpClient.get<Users[]>(
      this.baseApiUrl + '/Users/GetAllForPartner'
    );
  }

  insertProject(project: any): Observable<any> {
    const request: ProjectViewModel = {
      Id: null,
      Project_Name: project.name,
      CustomerId: project.customerId,
      BacklogLink: project.backloglink,
      PartnerId: project.partnerId,
      ProjectTypeId: project.projectType,
      Project_Code: project.projectCode,
      EndDate: this.datePipe.transform(project.endDate, 'yyyy-MM-dd'),
      StartDate: this.datePipe.transform(project.startDate, 'yyyy-MM-dd'),
      Description: project.descripion,
      Revenua: project.revenue,
      PM_Estimate: project.pmEstimate == '' ? null : project.pmEstimate,
      Brse_Estimate: project.brseEstimate == '' ? null : project.brseEstimate,
      Tester_Estimate:
        project.testerEstimate == '' ? null : project.testerEstimate,
      Comtor_Estimate:
        project.comtorEstimate == '' ? null : project.comtorEstimate,
      Developer_Estimate:
        project.developerEstimate == '' ? null : project.developerEstimate,
      UserPositions: [
        {
          PositionId: 1,
          User_Id: project.listPMs,
        },
        {
          PositionId: 2,
          User_Id: project.listMembers,
        },
      ],
      CustomerName: '',
      PartnerName: '',
      StatusCoding: true,
    };

    return this.httpClient.post<any>(
      this.baseApiUrl + '/Projects/CreateProject',
      request
    );
  }

  updateProject(project: any): Observable<any> {
    const request: ProjectViewModel = {
      Id: project.id,
      Project_Name: project.name,
      BacklogLink: project.backloglink,
      ProjectTypeId: project.projectType,
      Project_Code: project.projectCode,
      EndDate: this.datePipe.transform(project.endDate, 'yyyy-MM-dd'),
      StartDate: this.datePipe.transform(project.startDate, 'yyyy-MM-dd'),
      Description: project.descripion,
      Revenua: project.revenue,
      PM_Estimate: project.pmEstimate == '' ? null : project.pmEstimate,
      Brse_Estimate: project.brseEstimate == '' ? null : project.brseEstimate,
      Tester_Estimate:
        project.testerEstimate == '' ? null : project.testerEstimate,
      Comtor_Estimate:
        project.comtorEstimate == '' ? null : project.comtorEstimate,
      Developer_Estimate:
        project.developerEstimate == '' ? null : project.developerEstimate,
      UserPositions: [
        {
          PositionId: 1,
          User_Id: project.listPMs,
        },
        {
          PositionId: 2,
          User_Id: project.listMembers,
        },
      ],
      CustomerId: project.customerId,
      PartnerId: project.partnerId,
      CustomerName: '',
      PartnerName: '',
      StatusCoding: false,
    };

    return this.httpClient.put<any>(
      this.baseApiUrl + '/Projects/UpdateProject',
      request
    );
  }

  changeStatus(projectId: number, status: number): Observable<any> {
    status === 1 ? (status = 0) : (status = 1);

    return this.httpClient.patch<any>(
      this.baseApiUrl + '/Projects/ChangeStatus/' + projectId,
      status
    );
  }

  getDashboardStat(dateRef: any): Observable<any> {
    return this.httpClient.get(
      this.baseApiUrl + '/Projects/DashboardStat/' + dateRef
    );
  }
}
