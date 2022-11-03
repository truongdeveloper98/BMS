import { Component, HostListener, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { RecruitmentViewModel } from 'src/app/core/model/recruitment.model';
import { ReCruitmentService } from 'src/app/core/services/recruitment.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-create-recruitment',
  templateUrl: './create-recruitment.component.html',
  styleUrls: ['./create-recruitment.component.scss'],
})
export class CreateRecruitmentComponent implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<CreateRecruitmentComponent>,
    private recruitmentService: ReCruitmentService,
    private toastr: ToastrService
  ) {}

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
  }

  ngOnInit(): void {}

  saveChanges(recruit: RecruitmentViewModel) {
    this.recruitmentService
      .CreateReCruitment(recruit)
      .subscribe((result: any) => {
        this.toastr.open('Thêm mới tin tuyển dụng thành công');
        this.dialogRef.close();
      }),
      () => {
        this.toastr.error('Error:');
      };
  }
}
