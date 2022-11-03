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
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Project, ProjectTypes } from 'src/app/core/model/project';
import { ProjectTypeService } from 'src/app/core/services/project-type.service';
import { ProjectService } from 'src/app/core/services/project.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PaginationService } from 'src/app/shared/pagination.service';
import { ProjectComponent } from '../project/project.component';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss'],
})
export class ProjectListComponent implements OnInit {
  projects!: Project[];
  projectTypes!: ProjectTypes[];
  toggleFilter = false;

  roleName: string = '';
  isPm: number = 0;

  totalCount!: number;
  selectedItem: any;
  public screenHeight: any;

  displayedColumns: string[] = [
    'Project_Name',
    'Project_Code',
    'ProjectType_Name',
    'Description',
    'StartDate',
    'EndDate',
    'status',
    'actions',
  ];
  formFilter!: FormGroup;
  dataSource!: MatTableDataSource<Project>;

  constructor(
    private projectService: ProjectService,
    private projectTypeService: ProjectTypeService,
    private dialog: MatDialog,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    public paginationService: PaginationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const jwtHelper = new JwtHelperService();
    const decodedTokens = jwtHelper.decodeToken(localStorage.getItem('token')!);
    this.roleName =
      decodedTokens[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    this.isPm = +decodedTokens['isPm'];
    if (
      this.roleName !== 'SysAdmin' &&
      this.roleName !== 'Manager' &&
      this.isPm <= 0
    ) {
      this.router.navigate(['/admin/forbidden']);
    }

    this.formFilter = this.formBuilder.group({
      search: '',
      status: '',
      projectType: '',
      project: '',
    });

    this.projectTypeService.getProjectTypes().subscribe(
      (res) => {
        this.projectTypes = res;
      },
      (err) => {
        console.log(err);
      }
    );

    // const helper = new JwtHelperService();
    // const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    // this.roleName =
    //   decodedToken[
    //     'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    //   ];

    // this.isPm = +decodedToken['isPm'];

    this.GetProjects();
    this.selectedItem = null;
  }

  ngAfterViewInit() {
    var heightHeader = this.myHeightHeader.nativeElement.offsetHeight;
    var heightFilter = this.myHeightFilter.nativeElement.offsetHeight;

    setTimeout(() => {
      if (heightFilter > 0) {
        this.screenHeight =
          window.innerHeight -
          2 -
          86.5 -
          heightHeader -
          heightFilter -
          69.6 -
          8 -
          27;
      } else {
        this.screenHeight =
          window.innerHeight -
          2 -
          86.5 -
          heightHeader -
          heightFilter -
          69.6 -
          8;
      }
    }, 0);
  }

  @ViewChild(MatSort) matSort!: MatSort;
  @ViewChild('myHeightHeader') myHeightHeader!: ElementRef;
  @ViewChild('myHeightFilter') myHeightFilter!: ElementRef;

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    var heightHeader = this.myHeightHeader.nativeElement.offsetHeight;
    var heightFilter = this.myHeightFilter.nativeElement.offsetHeight;

    if (heightFilter > 0) {
      this.screenHeight =
        window.innerHeight -
        2 -
        86.5 -
        heightHeader -
        heightFilter -
        69.6 -
        8 -
        27;
    } else {
      this.screenHeight =
        window.innerHeight - 2 - 86.5 - heightHeader - heightFilter - 69.6 - 8;
    }
  }

  onToggleFilter() {
    this.toggleFilter = !this.toggleFilter;
    setTimeout(() => {
      var heightHeader = this.myHeightHeader.nativeElement.offsetHeight;
      var heightFilter = this.myHeightFilter.nativeElement.offsetHeight;

      if (heightFilter > 0) {
        this.screenHeight =
          window.innerHeight -
          2 -
          86.5 -
          heightHeader -
          heightFilter -
          69.6 -
          8 -
          27;
      } else {
        this.screenHeight =
          window.innerHeight -
          2 -
          86.5 -
          heightHeader -
          heightFilter -
          69.6 -
          8;
      }
    }, 0);
  }

  GetProjects() {
    this.projectService.getProjects(this.formFilter.value).subscribe(
      (res: any) => {
        this.projects = res.body;
        this.dataSource = new MatTableDataSource<Project>(this.projects);
        this.dataSource.sort = this.matSort;
        this.totalCount = res.headers.get('totalAmountOfRecords');
      },
      (err) => {
        console.log(err);
      }
    );
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
    this.GetProjects();
  }

  onCreate() {
    this.projectService.setEnable();
    this.projectService.initializeFormGroup();

    // const dialogConfig = new MatDialogConfig();
    // dialogConfig.disableClose = true;
    // dialogConfig.autoFocus = true;
    // dialogConfig.width = '60vw';
    // dialogConfig.maxHeight = '97vh';

    // this.dialog
    //   .open(ProjectComponent, dialogConfig)
    //   .afterClosed()
    //   .subscribe((result) => {
    //     this.GetProjects();
    //   });
    this.dialog
      .open(ProjectComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: { projectId: null },
      })
      .afterClosed()
      .subscribe((result) => {
        this.GetProjects();
      });
  }

  onEdit(id: any) {
    this.dialog
      .open(ProjectComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: { projectId: id },
      })
      .afterClosed()
      .subscribe((result) => {
        this.GetProjects();
      });
  }

  changeStatus(projectId: number, status: number) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Project',
          message:
            status === 0
              ? 'Xác nhận disable project này?'
              : 'Xác nhận enable project này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.projectService.changeStatus(projectId, status).subscribe(
            (res) => {
              this.toastr.open('Cập nhật trạng thái thành công!');
              this.GetProjects();
            },
            (err) => {
              this.toastr.error('Cập nhật trạng thái không thành công!');
              this.GetProjects();
            }
          );
        } //endif isConfirm
      });
  }

  onFilter() {
    this.GetProjects();
  }

  onReset() {
    this.formFilter.setValue({
      search: '',
      status: '',
      projectType: '',
      project: '',
    });

    this.GetProjects();
  }
}
