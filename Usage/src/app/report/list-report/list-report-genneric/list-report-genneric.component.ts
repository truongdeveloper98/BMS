import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  SimpleChange,
  ViewChild,
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { JwtHelperService } from '@auth0/angular-jwt';
import {
  ChangeStatusVm,
  CreateReportVm,
  PositionVm,
  ProjectVm,
  WorkingReportVm,
} from 'src/app/core/model/report.model';
import { ReportService } from 'src/app/core/services/report.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PaginationService } from 'src/app/shared/pagination.service';
import { environment } from 'src/environments/environment';

export const DateFormats = {
  parse: {
    dateInput: ['DD/MM/YYYY'],
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-list-report-genneric',
  templateUrl: './list-report-genneric.component.html',
  styleUrls: ['./list-report-genneric.component.scss'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE],
    },
    { provide: MAT_DATE_FORMATS, useValue: DateFormats },
  ],
})
export class ListReportGennericComponent implements OnInit {
  dataSource = new MatTableDataSource<any>();
  clickedRows = new Set<WorkingReportVm>();
  selectedItem!: any;
  selectedReports: number[] = [];
  inputHour: string = '';
  constructor(
    public dialog: MatDialog,
    private reportService: ReportService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private _formBuilder: FormBuilder,
    public paginationService: PaginationService
  ) { }
  @Input()
  addnew: any;
  @Input()
  manager: boolean = false;
  @Input()
  reports: WorkingReportVm[] = [];
  @Output()
  onUpdate = new EventEmitter<boolean>();
  @Output()
  onSendData = new EventEmitter<number[]>();
  form!: FormGroup;
  positions: PositionVm[] = [];
  projects: ProjectVm[] = [];
  @Input()
  reportType!: number;
  @Input()
  selected!: number;
  columnsToDisplay: any;
  reload = true;
  userID: string = '';
  index!: number;
  isEditableNew: boolean = true;
  isLoading = true;
  date!: Date;
  thongbao!: string;
  rates = [
    {
      name: '100%',
      value: 100,
    },

    {
      name: '150%',
      value: 150,
    },
    {
      name: '200%',
      value: 200,
    },
  ];

  workingtypes = [
    { name: 'Offline', value: 2 },
    { name: 'Remote', value: 0 },
    { name: 'Onsite', value: 3 },
  ];
  weekendDays!: Date;
  @ViewChild(MatSort) matSort!: MatSort;
  @Input() tabSelected!: number;

  ngOnInit(): void {
    if (this.manager === true) {
      if (this.reportType === 0) {
        this.columnsToDisplay = [
          'project',
          'displayname',
          'positionid',
          'workinghour',
          'day',
          'workingtype',
          'description',
          'status',
          'actions',
        ];
      }
      if (this.reportType === 1) {
        this.columnsToDisplay = [
          'project',
          'displayname',
          'positionid',
          'workinghour',
          'workingtype',
          'day',
          'rate',
          'description',
          'status',
          'actions',
        ];
      }
    } else {
      if (this.reportType === 0) {
        this.columnsToDisplay = [
          'project',
          'positionid',
          'workinghour',
          'day',
          'workingtype',
          'description',
          'status',
          'actions',
        ];
      }
      if (this.reportType === 1) {
        this.columnsToDisplay = [
          'project',
          'positionid',
          'workinghour',
          'workingtype',
          'day',
          'rate',
          'description',
          'status',
          'actions',
        ];
      }
    }
    this.form = this._formBuilder.group({
      items: this._formBuilder.array([]),
    });

    this.reportService.getPosition().subscribe((values) => {
      this.positions = values;
    });
    this.reportService.getProject().subscribe((values) => {
      this.projects = values;
      var found = this.projects.find((p : any) => p.ProjectId === environment.OtherProjectId);
      if(found === undefined) this.projects.push({ProjectId: environment.OtherProjectId, ProjectName: 'Other'});
      
    });
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    this.userID = decodedToken['UserID'];
    this.loadReport();
    this.selectedItem = null;
  }

  sortData(sort: Sort, form: any) {
    if (sort.direction === '') return;
    let isAsc: boolean = (sort.direction === 'asc');
    let data = form.get('items').value.sort((a: any, b: any) => {
      switch (sort.active) {
        case 'project': return (a.projectid > b.projectid ? 1 : -1) * (isAsc ? 1 : -1);
        case 'positionid': return (a.positionid > b.positionid ? 1 : -1) * (isAsc ? 1 : -1);
        case 'workinghour': return (a.workinghour > b.workinghour ? 1 : -1) * (isAsc ? 1 : -1);
        case 'day': return (a.workingday > b.workingday ? 1 : -1) * (isAsc ? 1 : -1);
        case 'workingtype': return (a.workingtype > b.workingtype ? 1 : -1) * (isAsc ? 1 : -1);
        case 'rate': return (a.rate > b.rate ? 1 : -1) * (isAsc ? 1 : -1);
        case 'displayname': return (a.displayname > b.displayname ? 1 : -1) * (isAsc ? 1 : -1);
        case 'status': return (a.status > b.status ? 1 : -1) * (isAsc ? 1 : -1);
        default: return 0;
      }
    });
  }

  ngOnChanges(changes: { [propName: string]: SimpleChange }) {
    if (
      changes['addnew'] &&
      changes['addnew'].previousValue != changes['addnew'].currentValue
    ) {
      if (this.addnew != 0 && this.selected === this.reportType) {
        this.AddNewRow();
      }
    }
    if (
      changes['reports'] &&
      changes['reports'].previousValue != changes['reports'].currentValue
    ) {
      this.loadReport();
    }
  }

  @HostListener('window:keyup.esc') onEscUp() {
    this.selectedReports = [];
    this.onSendData.emit(this.selectedReports);
  }

  loadReport() {
    let doAction = true;
    (this.form.get('items') as FormArray).controls.forEach(element => {
      if(element.get('isEditable')?.value === false) doAction = false;   
    });
    if(doAction){
      setTimeout(() => {
        this.form = this.formBuilder.group({
          items: this.formBuilder.array(
            this.reports.map((val) =>
              this.formBuilder.group({
                reportid: val.ReportId,
                workingday: val.Day,
                workinghour: [val.Time, { validators: Validators.required }],
                note: [val.Note, { validators: Validators.required }],
                ratevalue: [val.Rate, { validators: Validators.required }],
                projectid: [val.ProjectName, { validators: Validators.required }],
                positionid: [val.PositionName, { validators: Validators.required }],
                workingtype: [val.WorkingType, { validators: Validators.required }],
                displayname: [val.DisplayName],
                userId: [val.UserId, { validators: Validators.required }],
                status: val.Status,
                description: val.Description,
                isEditable: true,
                isNewRow: false,
              })
            )
          ),
        });

        this.dataSource = new MatTableDataSource(
          (this.form.get('items') as FormArray).controls
        );
        this.isLoading = false;
        this.dataSource.sort = this.matSort;
      }, 300)
    }
  }

  AddNewRow() {
    const control = this.form.get('items') as FormArray;
    control.insert(0, this.initiateForm());
    this.dataSource = new MatTableDataSource(control.controls);
    this.clickedRows.add(this.dataSource.filteredData[0]);
  }

  getValidity(form: any, i: any) {
    // if (form.get('items').at(i).get('isNewRow').value === true) {
    const date = new Date(form.get('items').at(i).get('workingday').value);
    if (date.getFullYear() < 2022) return true;
    if (date.getDay() === 0 || date.getDay() === 6) {
      form.get('items').at(i).get('ratevalue').patchValue(150);
    } else {
      form.get('items').at(i).get('ratevalue').patchValue(100);
    }
    // }
    if (form.get('items').at(i).get('workinghour').value <= 0 || form.get('items').at(i).get('workinghour').value > 8) return true;
    return (<FormArray>this.form.get('items')).controls[i].invalid;
  }

  initiateForm(): FormGroup {
    return this.formBuilder.group({
      reportid: '',
      workingday: ['', { validators: Validators.required }],
      workinghour: ['', { validators: Validators.required }],
      note: ['', { validators: Validators.required }],
      ratevalue: 0,
      projectid: [0, { validators: Validators.required }],
      positionid: [0, { validators: Validators.required }],
      workingtype: [2, { validators: Validators.required }],
      userId: [this.userID, { validators: Validators.required }],
      status: [0],
      isEditable: false,
      isNewRow: true,
    });
  }

  EditReport(VOFormElement: any, i: any) {
    VOFormElement.get('items').at(i).get('isEditable').patchValue(false);
    this.reportService
      .getReportById(VOFormElement.get('items').at(i).get('reportid').value)
      .subscribe((values) => {
        VOFormElement.get('items').at(i).patchValue({
          positionid: values.PositionId,
          projectid: values.ProjectId,
          reportid: values.ReportId,
          reporttype: this.reportType,
          workingday: values.Day,
          workinghour: values.Time,
          note: values.Note,
          workingtype: values.WorkingType,
          ratevalue: values.Rate,
          displayname: values.DisplayName,
        });
      });
  }

  approveReport(form: any, i: any) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Report',
          message: 'Xác nhận báo cáo này?',
          labelOK: 'Approve',
          labelCancel: 'Reject',
          secondDialog: true,
        },
      })
      .afterClosed()
      .subscribe(response => {
        if (response !== false) {
          let action = response === true ? 'Xác nhận ' : 'Reject ';
          const reportId = form.get('items').at(i).get('reportid').value;
          const model = new ChangeStatusVm();
          model.status = response === true ? 1 : 2;
          model.description = response;
          this.reportService.updateStatus(reportId, model).subscribe(
            () => {
              this.toastr.open(action + 'báo cáo thành công');
              form.get('items').at(i).get('description').patchValue(response);
              this.onUpdate.emit(true);
            },
            (err) => {
              if (err.error.code === 2 && err.status === 400) {
                this.toastr.error(err.error.message);
                this.onUpdate.emit(true);
              } 
              else this.toastr.error(action + 'báo cáo thất bại');
            }
          );
          
          
        } //endif response
      });
  }

  checkInputTime(form: any, i: any) {
    var s: string = form.get('items').at(i).get('workinghour').value;
    var n: number = +s;
    if (s !== '') {
      if (!isNaN(n) && n > 0 && n <= 8) {
        this.inputHour = s;
      }
      else {
        form.get('items').at(i).get('workinghour').patchValue(this.inputHour);
      }
    }
    else this.inputHour = '';
  }

  selectReport(id: any, event: any) {
    if (event.ctrlKey) {
      let sltdRp = this.selectedReports;
      let i: any;
      for (i = 0; i < sltdRp.length; i++) {
        if (sltdRp[i] === id) {
          sltdRp.splice(i, 1);
          this.selectedReports = sltdRp;
          this.onSendData.emit(this.selectedReports);
          return;
        }
      }
      sltdRp.push(id);
      this.selectedReports = sltdRp;
    }
    else {
      this.selectedReports = [id];
    }
    this.onSendData.emit(this.selectedReports);
  }

  SaveReport(form: any, i: any, element: any) {
    const reportVm: CreateReportVm = form.get('items').at(i).value;
    reportVm.reporttype = this.reportType;
    if (form.get('items').at(i).get('isNewRow').value === false) {
      this.reportService
        .edit(form.get('items').at(i).get('reportid').value, reportVm)
        .subscribe(
          () => {
            this.toastr.open('Cập nhật báo cáo thành công!');
            form.get('items').at(i).get('isEditable').patchValue(true);
            // this.onUpdate.emit(true); this.onUpdate.emit(true);
            this.clickedRows.delete(element);
            this.addnew = 0;
            var pr = this.projects.find(
              (x) =>
                x.ProjectId === form.get('items').at(i).get('projectid').value
            );
            var ps = this.positions.find(
              (x) =>
                x.PositionId === form.get('items').at(i).get('positionid').value
            );
            form
              .get('items')
              .at(i)
              .get('positionid')
              .patchValue(ps?.PositionName);
            form
              .get('items')
              .at(i)
              .get('projectid')
              .patchValue(pr?.ProjectName);
          },
          (err) => {
            if (err.error.code === 2 && err.status === 400) {
              this.toastr.error(err.error.message);
            } 
            else if (err.error.code === 5 && err.status === 400) {
              this.toastr.error(
                'Report đã được duyệt, không thể xoá/ sửa!'
              );
            } 
            else {
              this.toastr.error('Cập nhật báo cáo thất bại!');
            }
          }
        );
    }
    if (form.get('items').at(i).get('isNewRow').value === true) {
      this.reportService.createReport(reportVm).subscribe(

        () => {
          this.toastr.open('Thêm báo cáo thành công!');
          form.get('items').at(i).get('isEditable').patchValue(true);
          // this.onUpdate.emit(true); this.onUpdate.emit(true);
          this.clickedRows.delete(element);
          this.addnew = 0;
          var pr = this.projects.find(
            (x) =>
              x.ProjectId === form.get('items').at(i).get('projectid').value
          );
          var ps = this.positions.find(
            (x) =>
              x.PositionId === form.get('items').at(i).get('positionid').value
          );
          form
            .get('items')
            .at(i)
            .get('positionid')
            .patchValue(ps?.PositionName);
          form.get('items').at(i).get('projectid').patchValue(pr?.ProjectName);
          form.get('items').at(i).get('isNewRow').value = false;
          // window.location.reload();
          this.loadReport();
          this.onUpdate.emit(true);
        },
        (err) => {
          if (err.error.code === 2 && err.status === 400) {
            this.toastr.error(err.error.message);
          } 
          else if (err.error.code === 5 && err.status === 400) {
            this.toastr.error(
              'Report đã được duyệt, không thể xoá/ sửa!'
            );
          } 
          else {
            this.toastr.error('Thêm báo cáo thất bại!');
          }
        }
      );
    }

  }

  Cancel(form: any, i: any) {
    if (form.get('items').at(i).get('isNewRow').value === true) {
      const control = this.form.get('items') as FormArray;
      control.removeAt(i);
      this.dataSource = new MatTableDataSource(control.controls);
    } else {
      form.get('items').at(i).get('isEditable').patchValue(true);

      this.reportService
        .getReportById(form.get('items').at(i).get('reportid').value)
        .subscribe((values) => {
          var pr = this.projects.find(
            (x) => x.ProjectId === values.ProjectId
          );
          var ps = this.positions.find(
            (x) => x.PositionId === values.PositionId
          );
          // form.get('items').at(i).get('positionid').patchValue(ps?.PositionName);
          // form.get('items').at(i).get('projectid').patchValue(pr?.ProjectName);
          form.get('items').at(i).patchValue({
            positionid: ps?.PositionName,
            projectid: pr?.ProjectName,
            reportid: values.ReportId,
            reporttype: this.reportType,
            workingday: values.Day,
            workinghour: values.Time,
            note: values.Note,
            workingtype: values.WorkingType,
            ratevalue: values.Rate,
            displayname: values.DisplayName,
          });
        });
      
      this.addnew = 0;
      this.onUpdate.emit(true);
      this.loadReport();
    }
  }

  deleteReport(id: any) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Report',
          message: 'Xác nhận xoá báo cáo này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe(isConfirm => {
        if (isConfirm) {
          this.reportService.deleteReport(id).subscribe(
            () => {
              this.onUpdate.emit(true); this.onUpdate.emit(true);
              this.toastr.open('Xóa báo cáo thành công');
            },
            (err) => {
              if (err.error.code === 5 && err.status === 400) {
                this.toastr.error(
                  'Report đã được duyệt, không thể xoá/ sửa!'
                );
              } 
              else this.toastr.error('Xóa báo cáo thất bại');
            }
          );
          this.loadReport();
          this.onUpdate.emit(true);
        } //endif isConfirm
      });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
  }

  ViewDes(form: any, i: any) {
    const id = form.get('items').at(i).get('reportid').value;
    this.reportService.getReportById(id).subscribe((value) => {
      this.thongbao = value.Description;
    });
  }
}


