import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RecruitmentViewModel } from 'src/app/core/model/recruitment.model';
import { ReCruitmentService } from 'src/app/core/services/recruitment.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-edit-recruitment',
  templateUrl: './edit-recruitment.component.html',
  styleUrls: ['./edit-recruitment.component.scss'],
})
export class EditRecruitmentComponent implements OnInit {
  id: any;
  model: any;
  constructor(
    private dialogRef: MatDialogRef<EditRecruitmentComponent>,
    private recruitmentService: ReCruitmentService,
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.id = data.Id;
  }

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
  }

  ngOnInit(): void {
    this.recruitmentService.getById(this.id).subscribe((res) => {
      this.model = res;
    });
  }

  saveChanges(recruit: RecruitmentViewModel) {
    recruit.Id = this.id;
    this.recruitmentService.edit(recruit).subscribe(
      () => {
        this.toastr.open('Cập nhập tin tuyển dụng thành công!');
        this.dialogRef.close();
      },
      (error) => {
        this.toastr.error('Cập nhập tin tuyển dụng thất bại!');
      }
    );
  }
}
