import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppUserClaim } from '../core/model/app-user-claim';
import { HomeService } from '../core/services/home.service';
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
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

import { ChartType, ChartOptions } from 'chart.js';
import {
  SingleDataSet,
  Label,
  monkeyPatchChartJsLegend,
  monkeyPatchChartJsTooltip,
} from 'ng2-charts';
import { ProjectService } from '../core/services/project.service';
import { UsersService } from '../core/services/user.service';
import { ReportService } from 'src/app/core/services/report.service';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { PendingPopupComponent } from './pending-popup/pending-popup.component';
import { UserOnboardVM } from '../core/model/useronboard';
import { UserTypeViewModel } from '../core/model/userService.model';

import { ReCruitmentService } from '../core/services/recruitment.service';
import { PositionService } from '../core/services/position.service';
import { Positions } from '../core/model/project';
import { PaginationService } from '../shared/pagination.service';

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
interface pendingMemberModel {
  DisplayName: string;
  Email: string;
  PendingStart: string;
  EffortFree: string;
  CVLink: string;
}
interface pendingLevelModel {
  Name: string;
  Members: pendingMemberModel[];
}
interface pendingTeamModel {
  Team: string;
  MemberCount: number;
  expand: boolean;
  Levels: pendingLevelModel[];
}
interface pendingModel {
  Total: number;
  Team: pendingTeamModel[];
  expand: boolean;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})
export class DashboardComponent implements OnInit {
  //Các team sản xuất có department id < 10 để thuận tiện khi lọc.
  Departments = [
    {
      name: 'Division 1',
      value: 0,
    },
    {
      name: 'Division Faster',
      value: 1,
    },
    {
      name: 'Partner',
      value: 2,
    },
    {
      name: 'Admin',
      value: 11,
    },
    {
      name: 'HR',
      value: 12,
    },
    {
      name: 'Sale',
      value: 13,
    },
  ];

  Positions = [
    {
      name: 'Developer',
      value: 1,
    },
    {
      name: 'Tester',
      value: 2,
    },
    {
      name: 'BrSE',
      value: 3,
    },
    {
      name: 'Comtor',
      value: 4,
    },
    {
      name: 'Other',
      value: 5,
    },
    {
      name: 'Sale',
      value: 6,
    },
  ];

  Levels = [
    {
      name: 'Fresher',
      value: 1,
    },
    {
      name: 'Junior',
      value: 2,
    },
    {
      name: 'Middle Junior',
      value: 3,
    },
    {
      name: 'Senior',
      value: 4,
    },
    {
      name: 'Tech Lead',
      value: 5,
    },
    {
      name: 'Manager',
      value: 6,
    },
  ];

  onboardList:UserOnboardVM[]=[];

  permittedRole = ['Manager', 'SysAdmin', 'Sale', 'HR']
  statVisible = false;
  form!: FormGroup;
  userDetails: any;
  columnsToDisplay = ['team', 'level', 'displayname', 'pendingstart'];
  role: any;
  Types: UserTypeViewModel[]=[];

  pendingList: pendingModel = {Total: 0, Team: [], expand:false};

  formFilter!: FormGroup;
  workingHourStat: any = {
    TotalHour: 0,
    Other: {
      Hour: 0,
      Percentage: 0,
    },
    Project: {
      Hour: 0,
      Percentage: 0,
      Working: { Hour: 0, Percentage: 0 },
      OT: { Hour: 0, Percentage: 0 },
    },
  };
  public claims: AppUserClaim[] = [];

  public projectChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0,
  };
  public memberChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0,
    BeetSoft: 0,
    Partner: 0,
  };

  public deparmentChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0,
  };

  public statusRecruitmentChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0,
  };

  public statusRecruitmentPosChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0,
  };

  public memberOnboardChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0
  }

  public contractChart: any = {
    Labels: [],
    Data: [],
    Percent: [],
    Total: 0
  }

  public pieChartLabels: Label[] = [];
  public pieChartData: SingleDataSet = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];
  public chartColors: Array<any> = [
    {
      backgroundColor: [
        '#4bc0c0',
        '#9966ff',
        '#ff3784',
        '#36a2eb',
        '#f77825',
        '#cc2738',
        '#8b628a',
        '#8fbe00',
        '#ff3784',
        '#a5a6a7',
      ],
    },
  ];
  public pieChartOptions: ChartOptions = {
    legend: { position: 'left' },
    responsive: true,
    tooltips: {
      enabled: true,
      callbacks: {
        label: function (tooltipItem: any, data: any) {
          let label = data.labels[tooltipItem.index];
          let count =
            data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
          return label;
        },
      },
    },
    plugins: {
      datalabels: {
        formatter: (value: any, ctx: any) => {
          if (ctx.chart.data.labels) {
            return ctx.chart.data.labels[ctx.dataIndex];
          }
        },
      },
    },
  };
  position!: Positions[];
  recruitments: any;
  totalCount: any;

  constructor(
    private http: HttpClient,
    private homeService: HomeService,
    private projectService: ProjectService,
    private positionService: PositionService,
    private userService: UsersService,
    private reportService: ReportService,
    private recruitmentService: ReCruitmentService,
    private formBuilder: FormBuilder,
    private datePipe: DatePipe,
    public paginationService: PaginationService,
    private router: Router,
    private dialog: MatDialog
  ) {
    monkeyPatchChartJsTooltip();
    monkeyPatchChartJsLegend();
  }
  countRecruitmentStatus = 0;
  countRecruitmentPos = 0;

  ngOnInit(): void {
    this.formFilter = this.formBuilder.group({
      search: '',
      status: '',
      result: '',
      position: '',
      startdate: '',
      enddate: '',
    });    
    this.form = this.formBuilder.group({
      month: moment(),
    });    
    let dateRef = this.datePipe.transform(this.form.controls['month'].value, 'MMyyyy');     
    this.userService.getTypeList().subscribe((types) => {
      this.Types = types;
    })
    this.positionService.getPositions().subscribe((res) => {
      this.position = res;
    }); 
    this.getClaims();
    let helper = new JwtHelperService();
    let decodedToken = helper.decodeToken(localStorage.getItem('token')!);
    this.role =
      decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    if (this.permittedRole.find((x) => x === this.role))
      this.statVisible = true;
    if (this.statVisible) {           
      this.filterRecruitments(this.formFilter.value);
      this.formFilter.valueChanges.subscribe((values) => {
        this.filterRecruitments(values);
      });
      this.loadDashboardData(this.form.controls['month'].value);
    }
  }


  filterRecruitments(values: any) {
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    this.recruitmentService.getAll(values).subscribe((response: any) => {
      this.recruitments = response.body;
      this.totalCount = response.headers.get('totalAmountOfRecords');
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
    this.loadDashboardData(this.form.controls['month'].value);
  }

  loadDashboardData(value: any) {
    value = this.datePipe.transform(value, 'MMyyyy');
    this.projectChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0,
    };
    this.memberChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0,
      BeetSoft: 0,
      Partner: 0,
    };

    this.statusRecruitmentPosChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0,
      BeetSoft: 0,
      Partner: 0,
    };

    this.deparmentChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0
    }

    this.memberOnboardChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0
    }

    this.contractChart = {
      Labels: [],
      Data: [],
      Percent: [],
      Total: 0
    };

    this.projectService.getDashboardStat(value).subscribe((res) => {
      res.forEach((x: any) => {
        this.projectChart.Labels.push(x.Type);
        this.projectChart.Data.push(x.Count);
        this.projectChart.Total += x.Count;
      });
      res.forEach((x: any, index: any) => {
        this.projectChart.Labels[index] +=
          ': ' +
          this.projectChart.Data[index] +
          ' (' +
          (
            (this.projectChart.Data[index] * 100) /
            this.projectChart.Total
          ).toFixed(0) +
          '%) ';
      });
    });

    this.recruitmentService.getDashboardStat(value).subscribe((res) => {
      this.statusRecruitmentChart.Total = res.Total;
      res.Chartdata.forEach((x: any) => {
        if (x.Status === 0) {
          this.statusRecruitmentChart.Labels.push('Todo');
        } else {
          if (x.Status === 1) {
            this.statusRecruitmentChart.Labels.push('Doing');
          } else {
            if (x.Status === 2) {
              this.statusRecruitmentChart.Labels.push('Done');
            } else {
              this.statusRecruitmentChart.Labels.push('Cancel');
            }
          }
        }
        this.statusRecruitmentChart.Data.push(x.Count);
        this.countRecruitmentStatus = this.countRecruitmentStatus + 1;
      });
      res.Chartdata.forEach((x: any, index: any) => {
        this.statusRecruitmentChart.Labels[index] +=
          ': ' +
          this.statusRecruitmentChart.Data[index] +
          ' (' +
          (
            (this.statusRecruitmentChart.Data[index] /
              this.statusRecruitmentChart.Total) *
            100
          ).toFixed(0) +
          '%) ';
      });
    });

    this.recruitmentService.getDashboardStatpos(value).subscribe((res) => {
      this.statusRecruitmentPosChart.Total = res.Total;
      res.Chartdata.forEach((x: any) => {
        this.statusRecruitmentPosChart.Labels.push(x.Position);
        this.statusRecruitmentPosChart.Data.push(x.Count);
        this.countRecruitmentPos = this.countRecruitmentPos + 1;
      });
      res.Chartdata.forEach((x: any, index: any) => {
        this.statusRecruitmentPosChart.Labels[index] +=
          ': ' +
          this.statusRecruitmentPosChart.Data[index] +
          ' (' +
          (
            (this.statusRecruitmentPosChart.Data[index] /
              this.statusRecruitmentPosChart.Total) *
            100
          ).toFixed(0) +
          '%) ';
      });
    });
    this.userService.getMemberStat(value).subscribe((res) => {
      this.memberChart.Total = res.Total;
      this.memberChart.BeetSoft = res.BeetSoft;
      this.memberChart.Partner = res.Partner;
      res.Chartdata.forEach((x: any) => {
        this.memberChart.Labels.push(x.Role);
        this.memberChart.Data.push(x.Count);
      });
      res.Chartdata.forEach((x: any, index: any) => {
        this.memberChart.Labels[index] +=
          ': ' +
          this.memberChart.Data[index] +
          ' (' +
          (
            (this.memberChart.Data[index] * 100) /
            this.memberChart.Total
          ).toFixed(0) +
          '%) ';
      });

      this.deparmentChart.Total = res.Total;
      res.ChartDepartData.forEach((x: any) => {
        this.deparmentChart.Labels.push(
          this.Departments.find((item) => item.value == x.Department)?.name
        );
        this.deparmentChart.Data.push(x.Count);
      });
      res.ChartDepartData.forEach((x: any, index: any) => {
        this.deparmentChart.Labels[index] += ': ' + this.deparmentChart.Data[index] +  ' (' + (this.deparmentChart.Data[index] * 100 / this.deparmentChart.Total).toFixed(0) + '%) '
      })

      this.contractChart.Total = res.Total
      res.ChartContractData.forEach((x: any) => {
        this.contractChart.Labels.push(this.Types.find(item=>item.Id==x.Contract)?.Name)
        this.contractChart.Data.push(x.Count)
      })
      res.ChartContractData.forEach((x: any, index: any) => {
        this.contractChart.Labels[index] += ': ' + this.contractChart.Data[index] +  ' (' + (this.contractChart.Data[index] * 100 / this.contractChart.Total).toFixed(0) + '%) '
      })
    })

    this.homeService.getPendingMemberList(value).subscribe((res) => {
      this.pendingList.Team = res;
      this.pendingList.Total = 0;
      this.pendingList.Team.forEach((x: any) => {
        x.MemberCount = 0;
        x.expand = false;
        x.Levels.forEach((l: any) => {
          l.Members.forEach((m: any) => {
            x.MemberCount++;
          });
        });
        this.pendingList.Total += x.MemberCount;
      });
    });
    this.homeService.getWorkingHourStat(value).subscribe((res) => {
      this.workingHourStat = res
    })

    this.homeService.getMembersOnboard(value).subscribe((res) => {
      this.onboardList = res.OnboardList;
      this.memberOnboardChart.Total = this.onboardList.length;
      res.ChartMembersOnboardData.forEach((x: any) => {
        this.memberOnboardChart.Labels.push(this.Positions.find(item=>item.value==x.Position)?.name)
        this.memberOnboardChart.Data.push(x.Count)
      })
      res.ChartMembersOnboardData.forEach((x: any, index: any) => {
        this.memberOnboardChart.Labels[index] += ': ' + this.memberOnboardChart.Data[index] +  ' (' + (this.memberOnboardChart.Data[index] * 100 / this.memberOnboardChart.Total).toFixed(0) + '%) '
      })
    })
  }
  public getClaims = () =>{
    this.homeService.getClaims().subscribe(res => {
      this.claims = res as[];
    })
  }

  public getUser = () => {
    this.homeService.getUser().subscribe(
      (res) => {
        this.userDetails = res;
      },
      (err) => {
        console.log(err);
      }
    );
  };

  viewPendingDetails(members: any, team: any, level: any){
    this.dialog
      .open(PendingPopupComponent, {
        disableClose: true,
        width: '650px',
        // minHeight: '150px',
        data: {
          detail: members,
          title: team + ' ' + level,
        },
      })
      .afterClosed()
      .subscribe();
  }

  onFilter() {}
  viewCV(link: string) {
    if (link !== null) window.open(link, '_blank');
  }

  ExportPendingMember() {
    this.reportService.ExportPendingMember(
      this.datePipe.transform(this.form.controls['month'].value, 'MMyyyy')
    );
  }
}
