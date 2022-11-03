import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';

import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { PaginationService } from 'src/app/shared/pagination.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';

import { MatSort } from '@angular/material/sort';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UserOnboardVM } from 'src/app/core/model/useronboard';
import { UserOnboardService } from 'src/app/core/services/useronboard.service';
import { UseronboardComponent } from '../useronboard/useronboard.component';

@Component({
  selector: 'app-useronboard-list',
  templateUrl: './useronboard-list.component.html',
  styleUrls: ['./useronboard-list.component.scss'],
})
export class UseronboardListComponent implements OnInit {
  isLoading = true;
  selectedItem!: any;
  @Output()
  onUpdate = new EventEmitter<boolean>();
  usersonboard: any;
  form!: FormGroup;
  initialFormValues: any;
  totalAmountOfRecords: any;
  totalCount!: number;
  columnsToDisplay = [
    'fullname',
    'position',
    'language',
    'level',
    'onboarddate',
    'isdeleted',
    'actions',
  ];
  formFilter!: FormGroup;
  dataSource!: MatTableDataSource<UserOnboardVM>;
  levels = [
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
  positions = [
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

  constructor(
    private userOnboardService: UserOnboardService,
    private dialog: MatDialog,
    public paginationService: PaginationService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.formFilter = this.formBuilder.group({
      search: '',
    });
    this.GetUsersOnboard();
    this.formFilter.valueChanges.subscribe((values) => {
      this.GetUsersOnboard();
    });
  }

  @ViewChild(MatSort) matSort!: MatSort;
  onCreate() {
    this.dialog
      .open(UseronboardComponent, {
        disableClose: true,
        autoFocus: true,
        width: '45vw',
        maxHeight: '97vh',
        data: { Id: null },
      })
      .afterClosed()
      .subscribe((result) => {
        this.GetUsersOnboard();
      });
  }

  onEdit(id: any) {
    this.dialog
      .open(UseronboardComponent, {
        disableClose: true,
        autoFocus: true,
        width: '45vw',
        maxHeight: '97vh',
        data: { Id: id },
      })
      .afterClosed()
      .subscribe((result) => {
        this.GetUsersOnboard();
      });
  }

  GetUsersOnboard() {
    this.userOnboardService.getUsersOnboard(this.formFilter.value).subscribe(
      (res: any) => {
        this.isLoading = false;
        this.usersonboard = res.body;
        this.dataSource = new MatTableDataSource<UserOnboardVM>(
          this.usersonboard
        );
        this.dataSource.sort = this.matSort;
        this.totalCount = res.headers.get('totalAmountOfRecords');
      },
      (err) => {
        console.log(err);
      }
    );
  }

  changeUserOnboardStatus(memberId: number, isDeleted: boolean) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'User Onboard',
          message:
            isDeleted === false
              ? 'Xác nhận disable UserOnboard này?'
              : 'Xác nhận enable UsersOnboard này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.userOnboardService
            .changeUserOnboardStatus(memberId, isDeleted)
            .subscribe(
              (res) => {
                this.toastr.open('Cập nhật trạng thái thành công!');
                this.GetUsersOnboard();
              },
              (err) => {
                this.toastr.error('Cập nhật trạng thái không thành công!');
                this.GetUsersOnboard();
              }
            );
        } //endif isConfirm
      });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
  }
}
