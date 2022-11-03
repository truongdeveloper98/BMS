import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PartnerModel } from 'src/app/core/model/partner.model';
import { PartnerService } from 'src/app/core/services/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PaginationService } from 'src/app/shared/pagination.service';
import { EditPartnerComponent } from '../edit-partner/edit-partner.component';

@Component({
  selector: 'app-list-partner-cus',
  templateUrl: './list-partner-cus.component.html',
  styleUrls: ['./list-partner-cus.component.scss'],
})
export class ListPartnerCusComponent implements OnInit {
  @Input()
  dataSource!: MatTableDataSource<PartnerModel>;

  @Input()
  checkCus!: number;
  selectedItem!: any;
  isLoading = true;
  totalAmountOfRecords: any;
  columnsToDisplay = [
    'Customername',
    'Address',
    'Website',
    'Vote',
    'Note',
    'isdelete',
    'actions',
  ];

  constructor(
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private projectService: PartnerService,
    public paginationService: PaginationService
  ) {}

  public screenHeight: any;

  ngOnInit(): void {}

  @Output()
  onUpdate = new EventEmitter<boolean>();

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

  onEdit(id: any) {
    this.dialog
      .open(EditPartnerComponent, {
        disableClose: true,
        autoFocus: true,
        width: '60vw',
        maxHeight: '97vh',
        data: { Id: id, Check: this.checkCus },
      })
      .afterClosed()
      .subscribe((result) => {
        this.onUpdate.emit(true);
      });
  }
  changeStatus(id: number, status: boolean) {
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '450px',
        minHeight: '150px',
        data: {
          title: 'Partner',
          message:
            status === false
              ? 'Xác nhận disable partner này?'
              : 'Xác nhận enable partner này?',
          labelOK: 'Yes',
          labelCancel: 'No',
        },
      })
      .afterClosed()
      .subscribe((isConfirm) => {
        if (isConfirm) {
          this.projectService.changeStatus(id, status).subscribe(
            (res) => {
              this.onUpdate.emit(true);
            },
            (err) => {
              this.onUpdate.emit(false);
            }
          );
        } //endif isConfirm
      });
  }

  onFilter() {
    this.onUpdate.emit(true);
  }
  switchPage(event: PageEvent) {
    this.paginationService.change(event);
  }
}
