import { HttpResponse } from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';
import {
  userDTO,
  UserTypeViewModel,
} from 'src/app/core/model/userService.model';
import { PaginationService } from 'src/app/shared/pagination.service';
import { EditUsersComponent } from '../edit-users/edit-users.component';
import { RegisterUsersComponent } from '../register-users/register-users.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DataSource } from '@angular/cdk/collections';

@Component({
  selector: 'app-list-users',
  templateUrl: './list-users.component.html',
  styleUrls: ['./list-users.component.scss'],
})
export class ListUsersComponent implements OnInit {
  isLoading = true;
  selectedItem!: any;
  userTypelist!: UserTypeViewModel[];
  users!: userDTO[];
  dataSource!: MatTableDataSource<userDTO>;
  @ViewChild(MatSort) matSort!: MatSort;
  constructor(
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private userService: UsersService,
    private toastr: ToastrService,
    public dialog: MatDialog,
    public paginationService: PaginationService
  ) {}
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
  @Output()
  onUpdate = new EventEmitter<boolean>();

  form!: FormGroup;
  initialFormValues: any;
  totalAmountOfRecords: any;
  columnsToDisplay = [
    'Email',
    'DisplayName',    
    'Role',
    'Position',
    'Type',
    'Departments',
    'start_date',
    'end_date',
    'isdeleted',
    'actions',
  ];

  public screenHeight: any;

  ngOnInit(): void {
    this.userService.getTypeList().subscribe((res) => {
      this.userTypelist = res;
    });
    this.form = this.formBuilder.group({
      search: '',
      department: '-1'
    });
    this.initialFormValues = this.form.value;
    this.filterUsers(this.form.value);
    this.form.valueChanges.subscribe((values) => {
      this.filterUsers(values);
    });

    const connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:15084//toastr', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();
    connection
      .start()
      .then(function () {})
      .catch((err) => console.log('Error while starting connection: ' + err));

    connection.on('BroadcastMessage', () => {});
  }
  filterUsers(values: any) {
    values.page = this.paginationService.page;
    values.recordsPerPage = this.paginationService.pageCount;
    this.userService.getAll(values).subscribe((response: any) => {
      this.isLoading = false;
      this.users = response.body;
      this.dataSource = new MatTableDataSource<userDTO>(this.users);
      this.dataSource.sort = this.matSort;
      this.totalAmountOfRecords = response.headers.get('totalAmountOfRecords');
    });
  }

  openDialogRegister() {
    const dialogRef = this.dialog.open(RegisterUsersComponent, {
      disableClose: true,
      width: '70vw',
      maxHeight: '97vh',
      minHeight: '580px',
      data: {
        listDepartment: this.Departments,
      },
    });
    dialogRef.afterClosed().subscribe(() => {
      this.filterUsers(this.form.value);
    });
  }

  openDialog(id: string) {
    let dialogRef = this.dialog.open(EditUsersComponent, {
      disableClose: true,
      data: { Id: id, listDepartment: this.Departments },
      width: '65vw',
      maxHeight: '97vh',
      minHeight: '580px',
    });
    dialogRef.afterClosed().subscribe(() => {
      this.filterUsers(this.form.value);
    });
  }

  blockUser(id: string, blocked: boolean, email: string) {
    if (
      email === 'noreply@BeetSoft.vn' ||
      email === 'bms_admin@beetsoft.com.vn'
    ) {
      this.toastr.error('Không thể khoá tài khoản Admin!');
      return;
    }
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Member',
          message: blocked
            ? 'Xác nhận khoá member này?'
            : 'Xác nhận mở khoá member này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          if (blocked) {
            //block
            this.dialog
              .open(ConfirmDialogComponent, {
                disableClose: true,
                width: '450px',
                minHeight: '150px',
                data: {
                  title: 'Member',
                  message2: "Enter member's end date",
                  labelCancel: 'Disable',
                  secondDialog: true,
                  dialogIndex: 2,
                  inputtype: 'date',
                },
              })
              .afterClosed()
              .subscribe((res) => {
                if (res !== false) {
                  this.userService.changeStatus(id, res).subscribe(
                    () => {
                      this.toastr.open('Thay đổi trạng thái thành công!');
                    },
                    () => {
                      this.toastr.error('Thay đổi trạng thái thất bại!');
                    },
                    () => {
                      this.filterUsers(this.form.value);
                      this.onUpdate.emit(true);
                    }
                  );
                }
              });
          } else {
            //unblock
            this.userService.changeStatus(id, new Date()).subscribe(
              () => {
                this.toastr.open('Thay đổi trạng thái thành công!');
              },
              () => {
                this.toastr.error('Thay đổi trạng thái thất bại!');
              },
              () => {
                this.filterUsers(this.form.value);
                this.onUpdate.emit(true);
              }
            );
          }
        } //endif isConfirm
      });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
    this.filterUsers(this.form.value);
  }
}
