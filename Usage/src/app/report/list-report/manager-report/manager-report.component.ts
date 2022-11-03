import { DatePipe } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';

import {
  ChangeStatusVm,
  ProjectForReportVm,
  ReportOffVm,
  UserForReportVm,
  WorkingReportVm,
} from 'src/app/core/model/report.model';
import { ProjectService } from 'src/app/core/services/project.service';
import { ReportService } from 'src/app/core/services/report.service';
import { UsersService } from 'src/app/core/services/user.service';
import { PaginationService } from 'src/app/shared/pagination.service';
import { CreateReportComponent } from '../../create-report/create-report/create-report.component';
import {
  MomentDateAdapter,
  MAT_MOMENT_DATE_ADAPTER_OPTIONS,
} from '@angular/material-moment-adapter';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';

import * as _moment from 'moment';
import { default as _rollupMoment, Moment } from 'moment';
import { CheckReportComponent } from '../../check-report/check-report.component';
// import { ListReportGennericComponent } from '../list-report-genneric/list-report-genneric.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { environment } from 'src/environments/environment';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';


const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
  parse: {
    dateInput: 'MM/YYYY',
  },
  display: {
    dateInput: 'MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};
@Component({
  selector: 'app-manager-report',
  templateUrl: './manager-report.component.html',
  styleUrls: ['./manager-report.component.scss'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})
export class ManagerReportComponent implements OnInit {
  selectedReports: number[] = [];
  pageIndex = [1, 1, 1]
  revice: any;
  isPm: any;
  roleName: any;
  constructor(
    public datepipe: DatePipe,
    private formBuilder: FormBuilder,
    private reportService: ReportService,
    public paginationService: PaginationService,
    private userService: UsersService,
    public dialog: MatDialog,
    private datePipe: DatePipe,
    private toastr: ToastrService,
    private router: Router,
  ) { }
  form!: FormGroup;
  workingreports: any;
  overtimmereports: any;
  offreports: any;
  totalAmountOfRecordsWorking: any;
  totalAmountOfRecordsOff: any;
  selectedTabIndex!: number;
  projects: any;
  tabSelected = 0;
  selectedUsers: UserForReportVm[] = [];
  usersToDisplay: UserForReportVm[] = [];
  selectUser: UserForReportVm[] = [];


  ngOnInit(): void {
    //this.selectedReports = this.listReport.selectedReports;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);
    this.roleName =
      decodedToken[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    this.isPm = +decodedToken[
      'isPm'
    ];
    if(this.roleName !== 'SysAdmin' && this.roleName !== 'Manager' && this.isPm <= 0){
      this.router.navigate(['/admin/forbidden']);
    }
    this.form = this.formBuilder.group({
      department: '',
      projectid: '',
      displayName: '',
      month: moment(),
      status: '',
    });
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
    this.reportService.getProjectForReport().subscribe((values) => {
      this.projects = values;
    });
    this.userService.getUserForReport().subscribe((values) => {
      this.usersToDisplay = values;
    });

    this.form.get('displayName')?.valueChanges.subscribe((values) => {
      this.getUserOfProject();
    });

    this.form.valueChanges.subscribe((values) => {
      this.filterWorkingReports(values);
      this.filterReportOffs(values);
    });
  }

  chosenYearHandler(normalizedYear: Moment) {
    const ctrlValue = this.form.controls['month'].value;
    ctrlValue.year(normalizedYear.year());
    this.form.patchValue({ month: ctrlValue });
  }

  chosenMonthHandler(
    normalizedMonth: Moment,
    datepicker: MatDatepicker<Moment>
  ) {
    const ctrlValue = this.form.controls['month'].value;
    ctrlValue.year(normalizedMonth.year());
    ctrlValue.month(normalizedMonth.month());
    this.form.patchValue({ month: ctrlValue });
    datepicker.close();
  }

  onTabClick(event: any) {
    this.tabSelected = event;
    this.paginationService.setIndex(this.pageIndex[this.tabSelected])
    this.selectedReports = [];
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
  }

  update(event: any) {
    this.revice = event;
    if (this.revice == true) {
      this.filterWorkingReports(this.form.value);
      this.filterReportOffs(this.form.value);
    }
  }

  updateReportOff(event: any) {
    this.revice = event;
    if (this.revice == true) {
      this.filterWorkingReports(this.form.value);
      this.filterReportOffs(this.form.value);
    }
  }

  filterWorkingReports(values: any) {
    values.reportType = this.tabSelected;
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    values.month = this.datePipe.transform(values.month, 'yyyy-MM');
    if (this.tabSelected === 0) {
      this.reportService
        .managerReport(values)
        .subscribe((response: HttpResponse<WorkingReportVm[]>) => {
          this.workingreports = response.body;
          this.totalAmountOfRecordsWorking = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    }
    if (this.tabSelected === 1) {
      this.reportService
        .managerReport(values)
        .subscribe((response: HttpResponse<WorkingReportVm[]>) => {
          this.overtimmereports = response.body;
          this.totalAmountOfRecordsWorking = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    }
  }

  filterReportOffs(values: any) {
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    values.month = this.datePipe.transform(values.month, 'yyyy-MM');
    this.reportService
      .managerReportOf(values)
      .subscribe((response: HttpResponse<ReportOffVm[]>) => {
        this.offreports = response.body;
        this.totalAmountOfRecordsOff = response.headers.get(
          'totalAmountOfRecords'
        );
      });
  }

  switchPage(event: PageEvent) {
    // event.pageIndex = this.pageIndex[this.tabSelected]
    this.paginationService.change(event);
    this.pageIndex[this.tabSelected] = this.paginationService.page
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
  }

  CreateReport() {
    this.dialog
      .open(CreateReportComponent)
      .afterClosed()
      .subscribe(() => {
        this.filterWorkingReports(this.form.value);
        this.filterReportOffs(this.form.value);
      });
  }

  ExportProject() {
    const newstr = this.datePipe.transform(
      this.form.get('month')?.value,
      'MM-yyyy'
    );
    var date = newstr!.replace('-', '');
    this.reportService.ExportByProject(date);
  }

  ExportUser(isPartner: boolean) {
    const newstr = this.datePipe.transform(
      this.form.get('month')?.value,
      'MM-yyyy'
    );
    var time = newstr!.replace('-', '');
    this.reportService.ExportByUser(time, isPartner);
  }

  downloadFile(data: any) {
    const blob = new Blob([data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    const url = window.URL.createObjectURL(blob);
    window.open(url);
  }

  onCheckReport() {
    const date = this.datePipe.transform(this.form.controls['month'].value, 'MM-yyyy');
    var newstr = date!.replace("-", "");
    let projectId = this.form.get('projectid')?.value
    let popupTitle: string = '';
    if(projectId !== '')  popupTitle = ': ' + this.projects.find((x: any) => x.ProjectId === projectId)?.Name;
    if(projectId === '' || projectId === environment.OtherProjectId) projectId = '-1';
    this.dialog
      .open(CheckReportComponent, {
        disableClose: true,
        // width: '630px',
        maxWidth: '95vw',
        // minHeight: '275px',
        maxHeight: '97vh',
        data: {
          title: popupTitle,
          time: newstr,
          projectid: projectId
         },
      });
  }

  getSelectedReportList(event: any) {
    this.selectedReports = event;
  }

  approveSelected() {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Report',
          message: 'Xác nhận những báo cáo này?',
          labelOK: 'Approve',
          labelCancel: 'Cancel',
          //secondDialog: true,
        },
      })
      .afterClosed()
      .subscribe(response => {
        if (response === true) {
          let action = response === true ? 'Xác nhận ' : 'Reject ';
          // const model = new ChangeStatusVm();
          // model.status = response === true ? 1 : 2;
          // model.description = response;
          let model: number[] = [];
          this.selectedReports.forEach((x: any) => {
            model.push(x);
          });
          if (this.tabSelected === 2){
            this.reportService.multiApproveOff(model).subscribe(
              () => { this.toastr.open('Xác nhận báo cáo thành công'); this.updateReportOff(true);},
              (err) => {
                if (err.error.code === 2 && err.status === 400) {
                  this.toastr.error(err.error.message);
                  this.update(true);
                }
                else this.toastr.error('Xác nhận báo cáo thất bại');
              }
            );
          }
          else{
            this.reportService.multiApprove(model).subscribe(
              () => { this.toastr.open('Xác nhận báo cáo thành công'); this.update(true); },
              (err) => {
                if (err.error.code === 2 && err.status === 400) {
                  this.toastr.error(err.error.message);
                  this.update(true);
                }
                else this.toastr.error('Xác nhận báo cáo thất bại');
              }
            );
          }

        } // end if(response === true)
      }); //end suncribe()
  } //end function

  approveAll() {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Report',
          message: 'Xác nhận toàn bộ những báo cáo này?',
          labelOK: 'Approve',
          labelCancel: 'Cancel',
        },
      })
      .afterClosed()
      .subscribe(response => {
        if (response === true) {
          if (this.tabSelected === 0){
              let model: number[] = [];
              this.workingreports.forEach((x: any) => {
                model.push(x.ReportId);
              });
              this.reportService.multiApprove(model).subscribe(
                () => { this.toastr.open('Xác nhận báo cáo thành công!'); this.update(true);},
                (err) => {
                  if (err.error.code === 2 && err.status === 400) {
                    this.toastr.error(err.error.message);
                    this.update(true);
                  }
                  else this.toastr.error('Xác nhận báo cáo thất bại');
                }
              );
          }
          else if (this.tabSelected === 1){
            let model: number[] = [];
              this.overtimmereports.forEach((x: any) => {
                model.push(x.ReportId);
              });
              this.reportService.multiApprove(model).subscribe(
                () => { this.toastr.open('Xác nhận báo cáo thành công!'); this.update(true); },
                (err) => {
                  if (err.error.code === 2 && err.status === 400) {
                    this.toastr.error(err.error.message);
                    this.update(true);
                  }
                  else this.toastr.error('Xác nhận báo cáo thất bại');
                }
              );
          }
          else{
            let model: number[] = [];
              this.offreports.forEach((x: any) => {
                model.push(x.ReportOffId);
              });
              this.reportService.multiApproveOff(model).subscribe(
                () => { this.toastr.open('Xác nhận báo cáo thành công!'); this.updateReportOff(true);},
                (err) => {
                  if (err.error.code === 2 && err.status === 400) {
                    this.toastr.error(err.error.message);
                    this.update(true);
                  }
                  else this.toastr.error('Xác nhận báo cáo thất bại');
                }
              );
          }


        } // end if(isConfirm)
      });
  } //end function

  getUserOfProject(){
    let pjId = this.form.controls['projectid'].value;
    let name = this.form.controls['displayName'].value;
    if(name === "") name = "all_User_Off_Project";
    if(pjId === "" || pjId === environment.OtherProjectId){
      if(name === "all_User_Off_Project"){
        this.userService.getUserForReport().subscribe((values) => {
          this.usersToDisplay = values;
        });
      }
      else{
        this.userService.searchByName(name).subscribe((users) => {
          this.usersToDisplay = users;
        });
      }
    }
    else{
      this.reportService.getUserInProject(pjId, name).subscribe((users) => {
        this.usersToDisplay = users;
      });
    }
  }

} //end class
