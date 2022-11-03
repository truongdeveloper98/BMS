import { Component, HostListener, Inject, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CreateReportVm } from 'src/app/core/model/report.model';
import { ReportService } from 'src/app/core/services/report.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { DatePipe } from '@angular/common';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-create-report',
  templateUrl: './create-report.component.html',
  styleUrls: ['./create-report.component.scss'],
})
export class CreateReportComponent implements OnInit {
  constructor(
    private reportService: ReportService,
    private formBuilder: FormBuilder,
    private date: DatePipe,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<CreateReportComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.report = data.report;
  }
  @HostListener('window:keyup.esc') onKeyUp() {
    this.dialogRef.close();
  }
  report!: number;
  form!: FormGroup;
  formCreate!: FormGroup;
  positions: any;
  projects: any;
  createReport!: number;
  @Input()
  selectedReport: any;
  reset!: boolean;

  isSaveChange:boolean = false;
  selectCreateReports = [
    {
      name: 'Working Report',
      value: 0,
    },

    {
      name: 'Overtime Report',
      value: 1,
    },
    {
      name: 'Off Report',
      value: 2,
    },
  ];
  ngOnInit(): void {
    this.form = this.formBuilder.group({
      reporttype: 0,
    });
    this.reportService.getPosition().subscribe((values) => {
      this.positions = values;
    });
    this.reportService.getProject().subscribe((values) => {
      this.projects = values;
      var found = this.projects.find((p : any) => p.ProjectId === environment.OtherProjectId);
      if(found === undefined) this.projects.push({ProjectId: environment.OtherProjectId, ProjectName: 'Other'});
    });
    this.form.setValue({
      reporttype: this.report,
    });
    this.createReport = this.form.get('reporttype')?.value;
    this.form.get('reporttype')?.valueChanges.subscribe((value) => {
      this.createReport = value;
    });
  }
  CloseDialog() {
    this.dialogRef.close();
  }
  getDateArray(
    start: Date,
    end: Date,
    thuhai: boolean,
    thuba: boolean,
    thutu: boolean,
    thunam: boolean,
    thusau: boolean
  ) {
    var arr = new Array(),
      dt = new Date(start);
    while (dt <= end) {
      arr.push(new Date(dt));
      dt.setDate(dt.getDate() + 1);
    }
    arr = arr.filter((x) => x.getDay() !== 0);
    arr = arr.filter((x) => x.getDay() !== 6);
    if (thuhai === false) {
      arr = arr.filter((x) => x.getDay() !== 1);
    }
    if (thuba === false) {
      arr = arr.filter((x) => x.getDay() !== 2);
    }
    if (thutu === false) {
      arr = arr.filter((x) => x.getDay() !== 3);
    }
    if (thunam === false) {
      arr = arr.filter((x) => x.getDay() !== 4);
    }
    if (thusau === false) {
      arr = arr.filter((x) => x.getDay() !== 5);
    }
    return arr;
  }
  SaveWorkingReport(createReport: CreateReportVm) {
    createReport.workingdays = this.getDateArray(
      createReport.startdate,
      createReport.enddate,
      createReport.thuhai,
      createReport.thuba,
      createReport.thutu,
      createReport.thunam,
      createReport.thusau
    );
    createReport.reporttype = this.createReport;
    this.reportService.createReports(createReport).subscribe(
      () => {
        this.toastr.open('Thêm mới báo cáo thành công!');
        this.reset = true;
        this.isSaveChange = true;
        this.CloseDialog();
      },
      (err) => {
        this.isSaveChange = false;
        if (err.error.code === 2 && err.status === 400) {
          this.toastr.error(err.error.message);
        } else {
          this.toastr.error('Thêm mới báo cáo thất bại!');
        }
      }
    );
  }
}
