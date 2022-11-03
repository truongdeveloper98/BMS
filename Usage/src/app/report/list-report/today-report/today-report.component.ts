import { HttpResponse } from '@angular/common/http';
import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { ReportOffVm, WorkingReportVm } from 'src/app/core/model/report.model';
import { ReportService } from 'src/app/core/services/report.service';
import { UsersService } from 'src/app/core/services/user.service';
import { PaginationService } from 'src/app/shared/pagination.service';
import { CreateReportComponent } from '../../create-report/create-report/create-report.component';

@Component({
  selector: 'app-today-report',
  templateUrl: './today-report.component.html',
  styleUrls: ['./today-report.component.scss'],
})
export class TodayReportComponent implements OnInit {
  constructor(
    private formBuilder: FormBuilder,
    private reportService: ReportService,
    private userService: UsersService,
    public paginationService: PaginationService,
    public dialog: MatDialog
  ) { }
  currentTab!: number;
  form!: FormGroup;
  workingreports: any;
  overreports: any;
  revice: any;
  offreports: any;
  tabSelected = 0;
  pageIndex = [1, 1, 1]
  totalAmountOfRecordsWorking: any;
  totalAmountOfRecordsOff: any;
  selectedTabIndex!: number;
  addnew = 0;
  dayOffData: any = {
    Avaiable: 0,
    LastYearRemain: 0,
    Occupied: 0,
    ThisYearTotal: 0
  };
  ngOnInit(): void {
    this.form = this.formBuilder.group({
      search: '',
    });
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
    this.form.valueChanges.subscribe((values) => {
      this.filterWorkingReports(values);
      this.filterReportOffs(values);
    });

    this.userService.myLeaveDayData().subscribe((res) => {
      this.dayOffData = res;
    });
  }

  ngAfterViewInit() {
  }

  @ViewChild('myHeightHeader') myHeightHeader!: ElementRef;
  @ViewChild('myHeightPaginatorWorking') myHeightPaginatorWorking!: ElementRef;
  @ViewChild('myHeightPaginatorOverTime') myHeightPaginatorOverTime!: ElementRef;
  @ViewChild('myHeightPaginatorOff') myHeightPaginatorOff!: ElementRef;

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
  }

  AddNewRow() {
    this.addnew = this.addnew + 1;
    this.currentTab = this.tabSelected;
  }
  onTabClick(event: any) {
    this.tabSelected = event;
    this.paginationService.setIndex(this.pageIndex[this.tabSelected])
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
    this.userService.myLeaveDayData().subscribe((res) => {
      this.dayOffData = res;
    });
  }

  update(event: any) {
    this.revice = event;
    if (this.revice == true) {
      this.filterWorkingReports(this.form.value);
      this.filterReportOffs(this.form.value);
    }
    this.userService.myLeaveDayData().subscribe((res) => {
      this.dayOffData = res;
    });
  }
  filterWorkingReports(values: any) {
    values.reportType = this.tabSelected;
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    if (this.tabSelected === 0) {
      this.reportService
        .getWokingReportToday(values)
        .subscribe((response: HttpResponse<WorkingReportVm[]>) => {
          this.workingreports = response.body;
          this.totalAmountOfRecordsWorking = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    }
    if (this.tabSelected === 1) {
      this.reportService
        .getWokingReportToday(values)
        .subscribe((response: HttpResponse<WorkingReportVm[]>) => {
          this.overreports = response.body;
          this.totalAmountOfRecordsWorking = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    }
  }
  filterReportOffs(values: any) {
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    this.reportService
      .getOffReportToday(values)
      .subscribe((response: HttpResponse<ReportOffVm[]>) => {
        this.offreports = response.body;
        this.totalAmountOfRecordsOff = response.headers.get(
          'totalAmountOfRecords'
        );
      });
      this.userService.myLeaveDayData().subscribe((res) => {
        this.dayOffData = res;
      });
  }
  switchPage(event: PageEvent) {
    this.paginationService.change(event);
    this.pageIndex[this.tabSelected] = this.paginationService.page
    this.filterWorkingReports(this.form.value);
    this.filterReportOffs(this.form.value);
  }
  CreateReport() {
    this.dialog
      .open(CreateReportComponent, {
        disableClose: true,
        width: '850px',
        minHeight: '375px',
        // maxHeight: '543px',
        data: { report: this.tabSelected },
      })
      .afterClosed()
      .subscribe(() => {
        this.filterWorkingReports(this.form.value);
        this.filterReportOffs(this.form.value);
        this.userService.myLeaveDayData().subscribe((res) => {
          this.dayOffData = res;
        });
      });
  }
}
