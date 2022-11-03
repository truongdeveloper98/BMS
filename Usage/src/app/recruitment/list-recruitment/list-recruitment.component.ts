import {
  Component,
  ElementRef,
  HostListener,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Positions } from 'src/app/core/model/project';
import { RecruitmentViewModel } from 'src/app/core/model/recruitment.model';
import { PositionService } from 'src/app/core/services/position.service';
import { ReCruitmentService } from 'src/app/core/services/recruitment.service';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PaginationService } from 'src/app/shared/pagination.service';
import { CreateRecruitmentComponent } from '../create-recruitment/create-recruitment.component';
import { EditRecruitmentComponent } from '../edit-recruitment/edit-recruitment.component';

@Component({
  selector: 'app-list-recruitment',
  templateUrl: './list-recruitment.component.html',
  styleUrls: ['./list-recruitment.component.scss'],
})
export class ListRecruitmentComponent implements OnInit {
  screenHeight!: number;
  isLoading!: boolean;
  @Input()
  display = true;
  constructor(
    private recruitmentService: ReCruitmentService,
    private projectService: PositionService,
    private dialog: MatDialog,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    public paginationService: PaginationService
  ) {}
  formFilter!: FormGroup;
  position!: Positions[];
  totalCount!: number;
  selectedItem: any;

  displayedColumns: string[] = [
    'PositionName',
    'LanguageName',
    'FrameworkName',
    'DatePublish',
    'DateOnBroad',
    'Quantity',
    'Salary',
    'Result',
    'Priority',
    'Status',
    'actions',
  ];
  dataSource!: MatTableDataSource<RecruitmentViewModel>;
  recruitments!: RecruitmentViewModel[];
  ngOnInit(): void {
    this.projectService.getPositions().subscribe((res) => {
      this.position = res;
    });
    this.formFilter = this.formBuilder.group({
      search: '',
      status: '',
      result: '',
      position: '',
    });
    this.filterRecruitments(this.formFilter.value);
    this.formFilter.valueChanges.subscribe((values) => {
      this.filterRecruitments(values);
    });
  }
  filterRecruitments(values: any) {
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    this.recruitmentService.getAll(values).subscribe((response: any) => {
      this.isLoading = false;
      this.recruitments = response.body;

      this.dataSource = new MatTableDataSource<RecruitmentViewModel>(
        this.recruitments
      );
      this.dataSource.sort = this.matSort;
      this.totalCount = response.headers.get('totalAmountOfRecords');
    });
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
  toggleFilter = false;

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

  onCreate() {
    this.dialog
      .open(CreateRecruitmentComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: { projectId: null },
      })
      .afterClosed()
      .subscribe((result) => {
        this.filterRecruitments(this.formFilter.value);
      });
  }

  onEdit(id: any) {
    this.dialog
      .open(EditRecruitmentComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: { Id: id },
      })
      .afterClosed()
      .subscribe((result) => {
        this.filterRecruitments(this.formFilter.value);
      });
  }

  changeStatus(projectId: number, status: number) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Recruitment',
          message:
            status === 0
              ? 'Xác nhận hoàn tất tin này?'
              : 'Xác nhận chưa hoàn tất tin này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.recruitmentService.changeStatus(projectId, status).subscribe(
            (res) => {
              this.toastr.open('Cập nhật trạng thái thành công!');
              this.filterRecruitments(this.formFilter.value);
            },
            (err) => {
              this.toastr.error('Cập nhật trạng thái không thành công!');
              this.filterRecruitments(this.formFilter.value);
            }
          );
        } //endif isConfirm
      });
  }

  changeStatus_Re(projectId: number, status: number) {
    if (status === 0) {
      status = 1;
    } else {
      if (status === 1) {
        status = 2;
      } else {
        if (status === 2) {
          status = 3;
        } else {
          status = 0;
        }
      }
    }
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Recruitment',
          message: 'Xác nhận thay đổi trạng thái tin này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.recruitmentService.changeStatus_Re(projectId, status).subscribe(
            (res) => {
              this.toastr.open('Cập nhật trạng thái thành công!');
              this.filterRecruitments(this.formFilter.value);
            },
            (err) => {
              this.toastr.error('Cập nhật trạng thái không thành công!');
              this.filterRecruitments(this.formFilter.value);
            }
          );
        } //endif isConfirm
      });
  }

  changePro(projectId: number, status: number) {
    if (status === 0) {
      status = 1;
    } else {
      if (status === 1) {
        status = 2;
      } else {
        if (status === 2) {
          status = 0;
        }
      }
    }
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Recruitment',
          message: 'Bạn xác nhận thay đổi mức độ ưu tiên cho tin này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.recruitmentService.changeStatusPro(projectId, status).subscribe(
            (res) => {
              this.toastr.open('Cập nhật trạng thái thành công!');
              this.filterRecruitments(this.formFilter.value);
            },
            (err) => {
              this.toastr.error('Cập nhật trạng thái không thành công!');
              this.filterRecruitments(this.formFilter.value);
            }
          );
        } //endif isConfirm
      });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
    this.filterRecruitments(this.formFilter.value);
  }
  onFilter() {
    this.filterRecruitments(this.formFilter.value);
  }
  onReset() {
    this.formFilter.setValue({
      search: '',
      status: '',
      projectType: '',
      project: '',
    });

    this.filterRecruitments(this.formFilter.value);
  }
}
