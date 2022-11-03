import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormSalaryComponent } from '../form-salary/form-salary.component';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { PaginationService } from 'src/app/shared/pagination.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { SalaryInfo } from 'src/app/core/model/setting.model';
import { SalaryService } from 'src/app/core/services/salary.service';
import { MatSort } from '@angular/material/sort';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-list-salaries',
  templateUrl: './list-salaries.component.html',
  styleUrls: ['./list-salaries.component.scss']
})
export class ListSalariesComponent implements OnInit {
  isLoading = true;
  selectedItem!: any;
  @Output()
  onUpdate = new EventEmitter<boolean>();
  salaries: any;
  form!: FormGroup;
  initialFormValues: any;
  totalAmountOfRecords: any;
  totalCount!: number;
  columnsToDisplay = [
    'email',
    'displayname',
    'hourlysalary',
    'effectivedate',
    'isdeleted',
    'actions',
  ];
  formFilter!: FormGroup;
  dataSource!: MatTableDataSource<SalaryInfo>;
  constructor(private salaryService: SalaryService,private dialog: MatDialog,
    public paginationService: PaginationService,private formBuilder: FormBuilder,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.formFilter = this.formBuilder.group({

    });
    this.GetSalaries();
  }
  @ViewChild(MatSort) matSort!: MatSort
  onCreate() {    
    this.dialog
      .open(FormSalaryComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: {salaryId: null}
      })
      .afterClosed()
      .subscribe((result) => {
      this.GetSalaries();
      });
  }

  onEdit(id: any) {    
    this.dialog
      .open(FormSalaryComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: {Id: id}
      })
      .afterClosed()
      .subscribe((result) => {
      this.GetSalaries();
      });
  }

  GetSalaries() {
    this.salaryService.getSalaries(this.formFilter.value).subscribe(
      (res: any) => {
        this.isLoading = false;
        this.salaries = res.body;
        this.dataSource = new MatTableDataSource<SalaryInfo>(this.salaries);
        this.dataSource.sort = this.matSort;

        this.totalCount = res.headers.get('totalAmountOfRecords');
      },
      (err) => {
        console.log(err);
      }
    );
  }

  changeStatus(salaryId: number, isDeleted: boolean) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Salary',
          message: isDeleted === false ? 'Xác nhận disable salary này?' : 'Xác nhận enable salary này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe(isConfirm => {
        if (isConfirm) {
          this.salaryService.changeStatus(salaryId, isDeleted).subscribe(
            (res) => {
              this.toastr.open('Cập nhật trạng thái thành công!');
              this.GetSalaries();
            },
            (err) => {
              this.toastr.error('Cập nhật trạng thái không thành công!');
              this.GetSalaries();
            }
          );
        } //endif isConfirm
      });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
  }
}
