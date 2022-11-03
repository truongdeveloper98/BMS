import { Component, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from 'src/app/core/services/report.service';
import { PaginationService } from 'src/app/shared/pagination.service';

@Component({
  selector: 'app-check-report',
  templateUrl: './check-report.component.html',
  styleUrls: ['./check-report.component.scss']
})
export class CheckReportComponent implements OnInit {

  columnsToDisplay = [
    'email',
    'dislpayname',
    'missing'
  ];

  isLoading: boolean = true;

  dataSource!: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  title: string = 'Check reports'
  constructor(  
       @Inject(MAT_DIALOG_DATA) public data: any,
       public paginationService: PaginationService,
       public dialogRef: MatDialogRef<CheckReportComponent>,
       private reportService: ReportService,
       ) {
        if(data.title) this.title += data.title;
   }

  @HostListener('keyup', ['$event'])
  onkeyup(event:any) {
    if(event.keyCode === 27){
      this.dialogRef.close();
    }
  }

  GetReports(){
    this.reportService.checkReportOfUser(this.data.time, this.data.projectid).subscribe(
      (res) =>{
        this.dataSource = new MatTableDataSource<any>(res);
        this.dataSource.paginator = this.paginator;
        this.isLoading = false;
      }
    )
  }

  ngOnInit(): void {
    this.GetReports();
  }
  onClose() {
    this.dialogRef.close();
  }

}
