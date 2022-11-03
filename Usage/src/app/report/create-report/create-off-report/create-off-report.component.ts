import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { createReportOffVm, ReportOffVm } from 'src/app/core/model/report.model';
import { ReportService } from 'src/app/core/services/report.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-create-off-report',
  templateUrl: './create-off-report.component.html',
  styleUrls: ['./create-off-report.component.scss'],
})
export class CreateOffReportComponent implements OnInit {
  constructor(
    private formBuilder: FormBuilder,
    private reportService: ReportService,
    private userService: UsersService,
    private toasr: ToastrService,
    private dialogRef: MatDialogRef<CreateOffReportComponent>,
  ) {}

  dayOffData: any = {
    Avaiable: 0,
    LastYearRemain: 0,
    Occupied: 0,
    ThisYearTotal: 0
  };

  defaultTimeStart = [8, 0 , 0];
  defaultTimeEnd = [17, 0 , 0];
  nghiPhep: boolean = true;

  dateFilter = (d: Date | null): boolean => {
    return true;
    const day = (d || new Date()).getDay();
    const hour = (d || new Date()).getHours();
    const minute = (d || new Date()).getMinutes();
    //prevent off time
    let isWorkTime = !(hour < 8 || hour > 17 || (hour ===17 && minute > 0));
    //prevent lunch time
    let isLunchTime = !(hour === 12 && minute > 0);
    // Prevent Saturday and Sunday from being selected.
    return day !== 0 && day !== 6 && isLunchTime;
  };

  onDateSelected(form: any){
    if(!form.get('offdatestart').invalid && !form.get('offdateend').invalid){
      this.mindate = form.get('offdatestart').value;
      this.maxdate = form.get('offdateend').value;
      this.nghiPhep = this.caculateOffDay() <= this.dayOffData.Avaiable;
    }
    else{
      this.nghiPhep = true;
    }
    if(this.nghiPhep === false && form.get('offtype').value === 0)
        form.get('items').get('offtype').patchValue('');
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
          //nghi trong ngay
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
            if(startDate === true){
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

  form!: FormGroup;
  reset!: boolean;
  mindate!: Date;
  maxdate!: Date;
  offTypes: any


  ngOnInit(): void {
    this.userService.myLeaveDayData().subscribe((res) => {
      this.dayOffData = res;
    });

    this.reportService.getOffType().subscribe((res) =>{
      this.offTypes = res;
    });

    this.form = this.formBuilder.group({
      offtype: [0, { validators: Validators.required }],
      offdatestart: ['', { validators: Validators.required }],
      offdateend: ['', { validators: Validators.required }],
      note: ['', { validators: Validators.required }],
    });
    this.form.get('offdatestart')?.valueChanges.subscribe((values) => {
      this.mindate = values;
      this.nghiPhep = this.caculateOffDay() <= this.dayOffData.Avaiable;
      if(this.nghiPhep === false) this.form.get('offtype')?.patchValue('');
    });
    this.form.get('offdateend')?.valueChanges.subscribe((values) => {
      this.maxdate = values;
      this.nghiPhep = this.caculateOffDay() <= this.dayOffData.Avaiable;
      if(this.nghiPhep === false) this.form.get('offtype')?.patchValue('');
    });
  }

  SaveChange(event: any) {
    const offreportVm: createReportOffVm = this.form.value;
    this.reportService.createReportOff(offreportVm, this.caculateOffDay()).subscribe(
      () => {
        this.toasr.open('Thêm mới báo cáo thành công!');
        this.CloseDialog();
        this.form.patchValue({
          offtype: 0,
          offdatestart: ' ',
          offdateend: ' ',
          note: ' ',
        });
      },
      (err: any) => {
        if (err.error.code === 5 && err.status === 400) {
          this.toasr.error(
            'Report đã được duyệt, không thể xoá/ sửa!'
          );
        }
        else if (err.error.code === 2 && err.status === 400) {
          this.toasr.error(err.error.message);
        }
        else this.toasr.error('Cập nhập báo cáo thất bại!');
      }
    );
    this.userService.myLeaveDayData().subscribe((res) => {
      this.dayOffData = res;
    });
  }

  CloseDialog() {
    this.dialogRef.close();
  }
}
