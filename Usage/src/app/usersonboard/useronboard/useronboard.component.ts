import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UserOnboardService } from 'src/app/core/services/useronboard.service';
@Component({
  selector: 'app-useronboard',
  templateUrl: './useronboard.component.html',
  styleUrls: ['./useronboard.component.scss']
})
export class UseronboardComponent implements OnInit {
  memberId: string | null | undefined;
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

  constructor(public userOnboardService: UserOnboardService,
    public dialogRef: MatDialogRef<UseronboardComponent>,
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
      if(data.Id){
        this.memberId = data.Id;
        this.userOnboardService.getUserOnboard(this.memberId);        
      }
     }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.userOnboardService.form.valid) {
      //new UserOnboard
      if (!this.userOnboardService.form.get('id')?.value) {
        this.userOnboardService.insertUserOnboard(this.userOnboardService.form.value).subscribe(
          (result) => {
            this.toastr.open('Thêm mới thành viên onboard thành công!');

            this.userOnboardService.form.reset();
            this.userOnboardService.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Thêm mới thành viên onboard thất bại!');
          }
        );
      }
      //update salary
      else {
        this.userOnboardService.updateUserOnboard(this.userOnboardService.form.value).subscribe(
          (result) => {
            this.toastr.open('Cập nhật thành viên onboard thành công!');
            this.userOnboardService.form.reset();
            this.userOnboardService.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Cập nhật thành viên onboard thất bại!');
          }
        );
      }   
    }
  }

  onClose() {
    this.userOnboardService.form.reset();
    this.userOnboardService.initializeFormGroup();
    this.dialogRef.close();
  }

}
