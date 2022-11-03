import { DatePipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginationService } from 'src/app/shared/pagination.service';
import { environment } from 'src/environments/environment';
import {
  ChangeStatusVm,
  createReportOffVm,
  CreateReportVm,
  PositionVm,
  ProjectForReportVm,
  ProjectVm,
  ReportOffVm,
  UserForReportVm,
  WorkingReportVm,
} from '../model/report.model';
import * as FileSaver from 'file-saver';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private http: HttpClient, private datePipe: DatePipe, private paginationService: PaginationService, private toastr: ToastrService) { }
  private apiURL = environment.apiUrl + '/reports';
  getWokingReport(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<WorkingReportVm[]>(this.apiURL + '/GetMyReports', {
      observe: 'response',
      params,
    });
  }
  getWokingReportToday(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<WorkingReportVm[]>(
      this.apiURL + '/GetMyReportsToday',
      {
        observe: 'response',
        params,
      }
    );
  }
  managerReport(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<ReportOffVm[]>(this.apiURL + '/GetAllReportsUsers', {
      observe: 'response',
      params,
    });
  }
  managerReportOf(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<ReportOffVm[]>(
      this.apiURL + '/GetAllReportsOffUsers',
      {
        observe: 'response',
        params,
      }
    );
  }
  updateStatus(id: number, model: ChangeStatusVm) {
    const formData = this.buildFormStatus(model);
    return this.http.put(`${this.apiURL}/UpdateStatus/${id}`, formData);
  }

  multiApprove(model: number[]){
    return this.http.put(this.apiURL + '/ApproveAll/', model);
  }

  multiApproveOff(model: number[]){
    return this.http.put(this.apiURL + '/ApproveAllOff/', model);
  }

  updateStatusOff(id: number, model: ChangeStatusVm) {
    const formData = this.buildFormStatus(model);
    return this.http.put(`${this.apiURL}/UpdateStatusOff/${id}`, formData);
  }
  getOffReport(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<ReportOffVm[]>(this.apiURL + '/GetMyReportsOff', {
      observe: 'response',
      params,
    });
  }
  getOffReportToday(value: any): Observable<any> {
    const params = new HttpParams({ fromObject: value });
    return this.http.get<ReportOffVm[]>(
      this.apiURL + '/GetMyReportsOffToday',
      {
        observe: 'response',
        params,
      }
    );
  }
  edit(id: number, model: CreateReportVm) {
    const formData = this.buildFormData(model);
    return this.http.put(`${this.apiURL}/UpdateReport/${id}`, formData);
  }
  ExportByProject(date: any) {
    var currentDate = new Date();
    let monthYear = this.datePipe.transform(currentDate, 'MMyyyy');
    var fileName = 'Report_TimeSheet_' + date.toString() + '.xlsx';
    const url = this.apiURL + '/ExportByProject/' + date;
    return this.http.get(url, {
      // You need to set responseType to blob
      'responseType': 'blob'
    }).subscribe((x: Blob) => {
      FileSaver.saveAs(x, fileName);
    });
  }

  ExportByUser(time: any, isPartner: boolean) {
    var currentDate = new Date();
    let monthYear = this.datePipe.transform(currentDate, 'MMyyyy');
    var fileName = 'Report_TimeSheet_BeetSoft_' + time.toString() + '.xlsx';
    if(isPartner) fileName = 'Report_TimeSheet_Partner_' + time.toString() + '.xlsx';
    const url = this.apiURL + '/ExportReportTimeSheet/' + time + '/' + isPartner;
    return this.http.get(url, {
      // You need to set responseType to blob
      'responseType': 'blob'
    }).subscribe((x: Blob) => {
      if(x.size === 18) this.toastr.error('No data to export :(');
      else FileSaver.saveAs(x, fileName);
    });
  }

  ExportPendingMember(date: any) {
    var fileName = 'Report_PendingMember_' + date.toString() + '.xlsx';
    const url = this.apiURL + '/ExportPendingMemberList/' + date;
    return this.http.get(url, {
      // You need to set responseType to blob
      'responseType': 'blob'
    }).subscribe((x: Blob) => {
      FileSaver.saveAs(x, fileName);
    });
  }

  editReportOff(id: number, model: createReportOffVm, offDay: any) {
    const formData = this.buildFormDateOff(model, offDay);
    return this.http.put(`${this.apiURL}/UpdateReportOff/${id}`, formData);
  }
  deleteReport(id: number) {
    return this.http.put(`${this.apiURL}/DeleteReport/${id}`, '');
  }
  deleteReportOff(id: number) {
    return this.http.put(`${this.apiURL}/DeleteReportOff/${id}`, '');
  }
  getPosition(): Observable<PositionVm[]> {
    return this.http.get<PositionVm[]>(this.apiURL + '/GetPositionsForReport');
  }
  getProject(): Observable<ProjectVm[]> {
    return this.http.get<ProjectVm[]>(this.apiURL + '/GetProjectsByUser');
  }
  createReport(createReport: CreateReportVm) {
    const formData = this.buildFormData(createReport);
    return this.http.post(this.apiURL + '/CreateReport', formData);
  }
  createReports(createReport: CreateReportVm) {
    const formData = this.buildFormData(createReport);
    return this.http.post(this.apiURL + '/CreateMultiReports', formData);
  }
  getReportById(id: number): Observable<WorkingReportVm> {
    return this.http.get<WorkingReportVm>(this.apiURL + '/' + id);
  }
  getReportOffById(id: number): Observable<ReportOffVm> {
    return this.http.get<ReportOffVm>(this.apiURL + '/GetReportOff/' + id);
  }
  createReportOff(reportOff: createReportOffVm, offDay: any) {
    const formData = this.buildFormDateOff(reportOff, offDay);
    return this.http.post(
      environment.apiUrl + '/Reports/CreateReportOff',
      formData
    );
  }

  checkReportOfUser(time: string, projectid: number): Observable<any> {
    return this.http.get<any>(this.apiURL + '/CheckReports/' + time + '/' + projectid.toString());
  }

  getOffType(): Observable<any>{
    return this.http.get<any>(this.apiURL + '/GetOffType');
  }

  getProjectForReport(): Observable<ProjectForReportVm[]> {
    return this.http.get<ProjectForReportVm[]>(
      this.apiURL + '/GetProjectForReport'
    );
  }
  
  getUserInProject(projectId: number, name: any) :Observable<UserForReportVm[]>{
    return this.http
      .get<UserForReportVm[]>(this.apiURL + '/GetUserInProject/' + projectId + '/' + name);
  }

  private buildFormData(createReport: CreateReportVm): FormData {
    const formData = new FormData();
    if (createReport.workingtype) {
      formData.append('workingtype', createReport.workingtype.toString());
    }
    if (createReport.workingdays) {
      for (const day of createReport.workingdays) {
        formData.append(
          'workingdays',
          this.datePipe.transform(day, 'yyyy-MM-dd')!
        );
      }
    }
    if (createReport.description) {
      formData.append('description', createReport.description);
    }
    if (createReport.reportid) {
      formData.append('reportid', createReport.reportid.toString());
    }
    if (createReport.status) {
      formData.append('status', createReport.status.toString());
    }
    if (createReport.ratevalue) {
      formData.append('ratevalue', createReport.ratevalue.toString());
    }
    if (createReport.workinghour) {
      formData.append('workinghour', createReport.workinghour.toString());
    }
    if (createReport.positionid) {
      formData.append('positionid', createReport.positionid.toString());
    }
    if (createReport.reporttype) {
      formData.append('reporttype', createReport.reporttype.toString());
    }
    if (createReport.ratevalue) {
      formData.append('ratevalue', createReport.ratevalue.toString());
    }
    if (createReport.projectid) {
      formData.append('projectid', createReport.projectid.toString());
    }
    if (createReport.note) {
      formData.append('note', createReport.note);
    }
    if (createReport.workingday) {
      formData.append(
        'workingday',
        this.datePipe.transform(createReport.workingday, 'yyyy-MM-dd')!
      );
    }
    return formData;
  }

  private buildFormStatus(model: ChangeStatusVm): FormData {
    const formData = new FormData();
    if (model.description) {
      formData.append('description', model.description);
    }
    if (model.status) {
      formData.append('status', model.status.toString());
    }
    return formData;
  }

  private buildFormDateOff(reportOff: createReportOffVm, offDay: any): FormData {
    const formData = new FormData();
    if (reportOff.reportoffid) {
      formData.append('reportoffid', reportOff.reportoffid.toString());
    }
    if (reportOff.offdatestart) {
      formData.append(
        'offdatestart',
        this.datePipe.transform(reportOff.offdatestart, 'yyyy-MM-dd HH:mm')!
      );
    }
    if (reportOff.offdateend) {
      formData.append(
        'offdateend',
        this.datePipe.transform(reportOff.offdateend, 'yyyy-MM-dd HH:mm')!
      );
    }
    if (reportOff.description) {
      formData.append('description', reportOff.description);
    }
    if (reportOff.status) {
      formData.append('status', reportOff.status.toString());
    }
    if (reportOff.offtype) {
      formData.append('offtypeid', reportOff.offtype.toString());
    }
    if (reportOff.note) {
      formData.append('note', reportOff.note);
    }
    formData.append('OffDay', offDay);
    return formData;
  }
}
