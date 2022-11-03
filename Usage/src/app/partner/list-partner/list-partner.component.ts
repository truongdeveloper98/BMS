import { HttpResponse } from '@angular/common/http';
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
import { PartnerModel } from 'src/app/core/model/partner.model';
import { Project } from 'src/app/core/model/project';
import { PartnerService } from 'src/app/core/services/partner.service';
import { ProjectService } from 'src/app/core/services/project.service';
import { CreateRecruitmentComponent } from 'src/app/recruitment/create-recruitment/create-recruitment.component';
import { PaginationService } from 'src/app/shared/pagination.service';
import { CreatePartnerComponent } from '../create-partner/create-partner.component';

@Component({
  selector: 'app-list-partner',
  templateUrl: './list-partner.component.html',
  styleUrls: ['./list-partner.component.scss'],
})
export class ListPartnerComponent implements OnInit {
  tabSelected = 0;
  projectTypes: any;
  partners: any;
  totalCountProject!: number;
  pageIndex = [1, 1, 1];
  revice: any;
  roleName: any;
  isLoading!: boolean;
  totalAmountOfPartnerRecords: any;
  totalAmountOfCustomerRecords: any;
  constructor(
    private formBuilder: FormBuilder,
    public paginationService: PaginationService,
    private projectService: PartnerService,
    private router: Router,
    public dialog: MatDialog
  ) {}

  form!: FormGroup;
  isPm: any;
  dataSource!: MatTableDataSource<Project>;
  @ViewChild(MatSort) matSort!: MatSort;
  @ViewChild('myHeightHeader') myHeightHeader!: ElementRef;
  @ViewChild('myHeightPaginatorWorking') myHeightPaginatorWorking!: ElementRef;
  @ViewChild('myHeightPaginatorOverTime')
  myHeightPaginatorOverTime!: ElementRef;
  @ViewChild('myHeightPaginatorOff') myHeightPaginatorOff!: ElementRef;

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      search: '',
    });
    this.filterPartners(this.form.value);
    this.form.valueChanges.subscribe((values) => {
      this.filterPartners(values);
    });
  }

  filterPartners(values: any) {
    if (this.tabSelected === 0) {
      values.IsPartner = true;
      values.IsCustomer = false;
      values.page = this.paginationService.page;
      values.recordsPerPage = this.paginationService.pageCount;
      this.projectService
        .getAll(values)
        .subscribe((response: HttpResponse<PartnerModel[]>) => {
          this.isLoading = false;
          this.partners = response.body;
          this.totalAmountOfPartnerRecords = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    } else {
      values.IsCustomer = true;
      values.IsPartner = false;
      values.page = this.paginationService.page;
      values.recordsPerPage = this.paginationService.pageCount;
      this.projectService
        .getAll(values)
        .subscribe((response: HttpResponse<PartnerModel[]>) => {
          this.isLoading = false;
          this.partners = response.body;
          this.totalAmountOfCustomerRecords = response.headers.get(
            'totalAmountOfRecords'
          );
        });
    }
  }

  update(event: any) {
    this.revice = event;
    if (this.revice === true) {
      this.filterPartners(this.form.value);
    }
  }

  onTabClick(event: any) {
    this.tabSelected = event;
    this.filterPartners(this.form.value);
  }
  CreatePartner() {
    const dialogRef = this.dialog.open(CreatePartnerComponent, {
      disableClose: true,
      width: '850px',
      maxHeight: '97vh',
      data: { report: this.tabSelected },
    });
    dialogRef.afterClosed().subscribe(() => {
      this.filterPartners(this.form.value);
    });
  }

  switchPage(event: PageEvent) {
    this.paginationService.change(event);
    this.pageIndex[this.tabSelected] = this.paginationService.page;
    // this.GetProjects();
  }
}
