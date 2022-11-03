import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SettingService } from 'src/app/core/services/setting.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-create-framework',
  templateUrl: './create-framework.component.html',
  styleUrls: ['./create-framework.component.scss'],
})
export class CreateFrameworkComponent implements OnInit {
  other!: number;
  constructor(
    private formBuilder: FormBuilder,
    private settingService: SettingService,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<CreateFrameworkComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.other = data.other;
  }
  form!: FormGroup;

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: '',
    });
  }
  saveChanges() {
    let name = this.form.value;
    this.settingService
      .createOther(name, this.other)
      .subscribe((result: any) => {
        if (result.Succeeded === true) {
          this.toastr.open('Thêm mới thành công');
          this.dialogRef.close();
        } else {
          this.toastr.error('Error: ' + result.Errors[0].Code);
        }
      });
  }
}
