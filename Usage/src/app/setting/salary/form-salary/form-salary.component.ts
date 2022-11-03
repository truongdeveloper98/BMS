import { Component,  Inject,  OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Users } from 'src/app/core/model/project';
import { SalaryService } from 'src/app/core/services/salary.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-form-salary',
  templateUrl: './form-salary.component.html',
  styleUrls: ['./form-salary.component.scss']
})
export class FormSalaryComponent implements OnInit {

  users!: Users[];
  salaryId: string | null | undefined;

  constructor(public salaryService: SalaryService,
    public dialogRef: MatDialogRef<FormSalaryComponent>,
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
      if(data.Id){
        this.salaryId = data.Id;
        this.salaryService.getSalary(this.salaryId);
        
      }
     }

  ngOnInit(): void {
    this.salaryService.getUsers().subscribe(
      (res) => {
        this.users = res;
      },
      (err) => {
        console.log(err);
      }
    );
  }

  onSubmit() {
    if (this.salaryService.form.valid) {
      //new salary
      if (!this.salaryService.form.get('id')?.value) {
        this.salaryService.insertSalary(this.salaryService.form.value).subscribe(
          (result) => {
            this.toastr.open('Thêm mới lương thành công!');

            this.salaryService.form.reset();
            this.salaryService.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Thêm mới lương thất bại!');
          }
        );
      }
      //update salary
      else {
        this.salaryService.updateSalary(this.salaryService.form.value).subscribe(
          (result) => {
            this.toastr.open('Cập nhật lương thành công!');
            this.salaryService.form.reset();
            this.salaryService.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Cập nhật lương thất bại!');
          }
        );
      }   
    }
  }

  onClose() {
    this.salaryService.form.reset();
    this.salaryService.initializeFormGroup();
    this.dialogRef.close();
  }

}
