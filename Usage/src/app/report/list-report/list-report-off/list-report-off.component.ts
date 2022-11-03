import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  SimpleChange,
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { JwtHelperService } from '@auth0/angular-jwt';
import {
  ChangeStatusVm,
  createReportOffVm,
  ReportOffVm,
} from 'src/app/core/model/report.model';
import { ReportService } from 'src/app/core/services/report.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { Sort } from '@angular/material/sort';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'app-list-report-off',
  templateUrl: './list-report-off.component.html',
  styleUrls: ['./list-report-off.component.scss'],
})
export class ListReportOffComponent implements OnInit {
  clickedRows = new Set<ReportOffVm>();
  selectedReports: number[] = [];
  @Output()
  onUpdate = new EventEmitter<boolean>();
  @Input()
  models: any;
  @Input() addnew!: number;
  @Input() monthOffAvaiable!: number;
  @Input()
  manager: boolean = false;
  @Input()
  selected!: number;
  duration: number = 0;
  reportType = 2;
  dataSource = new MatTableDataSource<any>();
  isEditableNew: boolean = true;
  isLoading = true;
  columnsToDisplay: any;
  form!: FormGroup;
  mindate!: Date; maxdate!: Date;
  defaultTimeStart = [8, 0 , 0]; defaultTimeEnd = [17, 0 , 0];
  nghiPhep: boolean = true;
  @Input() tabSelected!: number;
  @Output()
  onSendData = new EventEmitter<number[]>();
  public screenHeight: any;
  userID: string = '';
  thongbao!: string;
  offTypes: any;

  constructor(
    private reportService: ReportService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private _formBuilder: FormBuilder,
    public dialog: MatDialog,
  ) { }

  @HostListener('window:keyup.esc') onEscUp() {
    //reset selected report list
    this.selectedReports = [];
    //send selected report list to parent component
    this.onSendData.emit(this.selectedReports);
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
      changes['models'] &&
      changes['models'].previousValue != changes['models'].currentValue
    ) {
      this.loadData();
    }
  }

  ngOnInit(): void {
    if (this.manager === true) {
      this.columnsToDisplay = [
        'displayname',
        'startdate',
        'enddate',
        'offtype',
        'note',
        'status',
        'actions',
      ];
    } else {
      this.columnsToDisplay = [
        'startdate',
        'enddate',
        'offtype',
        'note',
        'status',
        'actions',
      ];
    }

    this.reportService.getOffType().subscribe((res) =>{
      this.offTypes = res;
    });

    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    this.userID = decodedToken['UserID'];

    this.form = this._formBuilder.group({
      items: this._formBuilder.array([]),
    });
    this.loadData();
  }

  dateFilter = (d: Date | null): boolean => {
    return true;
    const day = (d || new Date()).getDay();
    const hour = (d || new Date()).getHours();
    const minute = (d || new Date()).getMinutes();
    //prevent off time
    let isWorkTime = !(hour < 8 || hour > 17 || (hour ===17 && minute > 0))
    //prevent lunch time
    let isLunchTime = !(hour === 12 && minute > 0)
    // Prevent Saturday and Sunday from being selected.
    return day !== 0 && day !== 6 && isLunchTime;
  };

  onDateSelected(form: any, i: any){
    if(!form.get('items').at(i).get('offdatestart').invalid)
      this.mindate = form.get('items').at(i).get('offdatestart').value;
    if(!form.get('items').at(i).get('offdateend').invalid)
      this.maxdate = form.get('items').at(i).get('offdateend').value;
    if(!form.get('items').at(i).get('offdatestart').invalid && !form.get('items').at(i).get('offdateend').invalid){
      this.mindate = form.get('items').at(i).get('offdatestart').value;
      this.maxdate = form.get('items').at(i).get('offdateend').value;
      this.nghiPhep = this.caculateOffDay() - this.duration <= this.monthOffAvaiable;
    }
    else{
      this.nghiPhep = true;
    }
    if(this.nghiPhep === false && form.get('items').at(i).get('offtype').value === 0)
        form.get('items').at(i).get('offtype').patchValue('');
  }

  caculateOffDay(): number{
    let min = new Date(this.mindate);
    if(min.getHours() < 8) {
      min.setHours(8);
      min.setMinutes(0);
    }
    let max = new Date(this.maxdate);
    if(max.getHours() >= 17) {
      max.setHours(17);
      max.setMinutes(0);
    }
    if(max > min){
      let dayOffCount = 0;
      let hours = 0;
      let minutes = 0;
      let minMinutes = 0
      let startDate = true;
      while(min <= max){
        if(min.getDate() === max.getDate() &&
           min.getMonth() === max.getMonth() &&
           min.getDay() !== 0 &&
           min.getDay() !== 6){
          hours += max.getHours() - min.getHours();
          minutes = max.getMinutes() - min.getMinutes();
          if(minutes < 0){
            hours = hours - 1;
            minutes += 60;
          }
          if(min.getHours() <= 12 && max.getHours() >= 13) hours = hours - 1;

        }

        else{
          //khong tinh ngay cuoi tuan
          if(min.getDay() !== 0 && min.getDay() !== 6){
            hours += 17 - min.getHours();
            if(min.getHours() <= 12) hours = hours- 1;
            if(startDate){
              minMinutes = 60 - min.getMinutes();
              if(minMinutes < 60) hours = hours- 1;
              else minMinutes = 0;
            }

          }
        }
        //ngay tiep theo
        min.setDate(min.getDate() + 1);
        startDate = false;
        if(min <= max){
          min.setHours(8);
          min.setMinutes(0);
        }


      }
      if(minutes + minMinutes >= 60){
        hours += 1;
        minutes = minutes + minMinutes - 60;
      }
      else minutes = minutes + minMinutes;
      dayOffCount = (hours + (minutes) / 60) / 8;
      return dayOffCount;
    }
    else return 99999;
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

  sortData(sort: Sort, form: any) {
    if (sort.direction === '') return;
    let isAsc: boolean = (sort.direction === 'asc');
    let data = form.get('items').value.sort((a: any, b: any) => {
      switch (sort.active) {
        case 'offtype': return (a.offtype > b.offtype ? 1 : -1) * (isAsc ? 1 : -1);
        case 'displayname': return (a.displayname > b.displayname ? 1 : -1) * (isAsc ? 1 : -1);
        case 'status': return (a.status > b.status ? 1 : -1) * (isAsc ? 1 : -1);
        default: return 0;
      }
    });
  }

  loadData() {
    let doAction = true;
    (this.form.get('items') as FormArray).controls.forEach(element => {
      if(element.get('isEditable')?.value === false) doAction = false;
    });
    if(doAction){
      setTimeout(() => {
        this.form = this.formBuilder.group({
          items: this.formBuilder.array(
            this.models.map(
              (val: {
                ReportOffId: any;
                OffTypeId: any;
                OffDateStart: any;
                OffDateEnd: any;
                Note: string;
                DisplayName: string;
                UserId: string;
                Status: number;
                Description: string;
              }) =>
                this.formBuilder.group({
                  reportid: val.ReportOffId,
                  offtype: [val.OffTypeId, { validators: Validators.required }],
                  offdatestart: [
                    val.OffDateStart,
                    { validators: Validators.required },
                  ],
                  offdateend: [val.OffDateEnd, { validators: Validators.required }],
                  note: [val.Note, { validators: Validators.required }],
                  displayname: val.DisplayName,
                  userId: [val.UserId, { validators: Validators.required }],
                  status: val.Status,
                  isEditable: true,
                  isNewRow: false,
                  describe: val.Description
                })

            )
          ),
        });


        this.dataSource = new MatTableDataSource(
          (this.form.get('items') as FormArray).controls
        );
        this.isLoading = false;
      }, 300);
    }
  }

  AddNewRow() {
    const control = this.form.get('items') as FormArray;
    control.insert(0, this.initiateForm());
    this.dataSource = new MatTableDataSource(control.controls);
    this.clickedRows.add(this.dataSource.filteredData[0]);
    this.mindate = new Date(2021, 1);
    this.maxdate = new Date(2050, 12);
  }

  initiateForm(): FormGroup {
    return this.formBuilder.group({
      reportid: '',
      offtype: ['', { validators: Validators.required }],
      offdatestart: ['', { validators: Validators.required }],
      note: ['', { validators: Validators.required }],
      offdateend: ['', { validators: Validators.required }],
      userId: [this.userID, { validators: Validators.required }],
      status: [0],
      isEditable: false,
      isNewRow: true,
    });
  }

  EditReport(VOFormElement: any, i: any) {
    this.mindate = VOFormElement.get('items').at(i).get('offdatestart').value;
    this.maxdate = VOFormElement.get('items').at(i).get('offdateend').value;
    if(VOFormElement.get('items').at(i).get('offtype').value === 0) this.duration = this.caculateOffDay();
    else this.duration = 0;
    // this.nghiPhep = this.caculateOffDay() <= this.monthOffAvaiable || VOFormElement.get('items').at(i).get('offtype').value === 0;
    // if(this.nghiPhep === false) VOFormElement.get('items').at(i).get('offtype').patchValue('');
    VOFormElement.get('items').at(i).get('isEditable').patchValue(false);
    this.reportService
      .getReportOffById(VOFormElement.get('items').at(i).get('reportid').value)
      .subscribe((values) => {
        VOFormElement.get('items').at(i).patchValue({
          offtype: values.OffTypeId,
          offdatestart: values.OffDateStart,
          offdateend: values.OffDateEnd,
          note: values.Note,
          displayname: values.DisplayName,
        });
      });
  }

  SaveReport(VOFormElement: any, i: any, element: any) {
    if (VOFormElement.get('items').at(i).get('isNewRow').value === false) {
      const reportVm: createReportOffVm =
        VOFormElement.get('items').at(i).value;
      reportVm.reportoffid = VOFormElement.get('items')
        .at(i)
        .get('reportid').value;
      this.reportService
        .editReportOff(
          VOFormElement.get('items').at(i).get('reportid').value,
          reportVm, this.caculateOffDay()
        )
        .subscribe(
          (success: any) => {
            this.toastr.open('Cập nhật báo cáo thành công!');
            this.clickedRows.delete(element);
            this.addnew = 0;
            VOFormElement.get('items').at(i).get('isEditable').patchValue(true);
            this.onUpdate.emit(true);
            this.loadData();
          },
          (err: any) => {
            if (err.error.code === 5 && err.status === 400) {
              this.toastr.error(
                'Report đã được duyệt, không thể xoá/ sửa!'
              );
            }
            else if (err.error.code === 2 && err.status === 400) {
              this.toastr.error(err.error.message);
            }
            else this.toastr.error('Cập nhập báo cáo thất bại!');
          }
        );
    }
    if (VOFormElement.get('items').at(i).get('isNewRow').value === true) {
      const reportVm: createReportOffVm =
        VOFormElement.get('items').at(i).value;
      this.reportService.createReportOff(reportVm, this.caculateOffDay()).subscribe(
        (success: any) => {
          this.toastr.open('Thêm mới báo cáo thành công!');
          // this.onUpdate.emit(true); this.onUpdate.emit(true);
          this.clickedRows.delete(element);
          this.addnew = 0;
          VOFormElement.get('items').at(i).get('isEditable').patchValue(true);
          // window.location.reload();
          this.onUpdate.emit(true); this.onUpdate.emit(true);
          this.loadData();
        },
        (err: any) => {
          if (err.error.code === 2 && err.status === 400) {
            this.toastr.error(err.error.message);
          }
          else this.toastr.error('Thêm mới báo cáo thất bại!');
        }
      );
    }
  }

  getValidity(form: any, i: any) {
    return (<FormArray>this.form.get('items')).controls[i].invalid;
  }


  Cancel(form: any, i: any) {
    if (form.get('items').at(i).get('isNewRow').value === true) {
      const control = this.form.get('items') as FormArray;
      control.removeAt(i);
      this.dataSource = new MatTableDataSource(control.controls);
    } else {

      this.onUpdate.emit(true);
      this.addnew = 0;
    }
  }

  ViewDes(form: any, i: any) {
    const id = form.get('items').at(i).get('reportid').value;
    this.reportService.getReportOffById(id).subscribe((value) => {
      this.thongbao = value.Description;
    });
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
          this.reportService.deleteReportOff(id).subscribe(
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
          this.onUpdate.emit(true);
          this.loadData();
        } //endif isConfirm
      });
  }

  approveReport(form: any, i: any) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Off report',
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
          this.reportService.updateStatusOff(reportId, model).subscribe(
            () => {
              this.toastr.open(action + 'báo cáo thành công');
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
          form.get('items').at(i).get('describe').patchValue(response);

        } //endif ressponse !== false
      });
  }
}
