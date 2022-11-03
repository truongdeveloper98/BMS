import { Component, HostListener, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  FormGroupDirective,
  Validators,
} from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';
import { ChangePwViewModel } from 'src/app/core/model/userService.model';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
})
export class ChangePasswordComponent implements OnInit {
  old = true;
  new = true;
  retype = true;

  changePwForm = this.formBuilder.group(
    {
      OldPassword: ['', [Validators.required]],
      NewPassword: ['', [
        Validators.required,
        Validators.pattern(
          '(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}'
        ),]
      ],
      RetypePassword: ['', Validators.required],
    },
    {
      validators: this.mustMatch('NewPassword', 'RetypePassword'),
    }
  );

  constructor(
    private formBuilder: FormBuilder,
    private userService: UsersService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void { }

  mustMatch(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (matchingControl.errors && !matchingControl.errors.mustMatch) {
        return;
      }

      // set error on matchingControl if validation fails
      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      } else {
        matchingControl.setErrors(null);
      }
      return null;
    };
  }

  onSubmit(formDirective: FormGroupDirective) {
    const request: ChangePwViewModel = {
      OldPassword: this.changePwForm.controls['OldPassword'].value,
      NewPassword: this.changePwForm.controls['NewPassword'].value,
    };

    this.userService.ChangePassword(request).subscribe(
      (res) => {
        formDirective.resetForm();

        this.toastr.open('Cập nhật thành công!');
      },
      (err) => {
        if (err.status == 400) {
          if (err.error.result == 1) {
            this.toastr.error('Cập nhật thất bại!');
          }

          if (err.error.result == 0) {
            this.changePwForm.get('OldPassword')?.setErrors({ validCkPw: true });
          }
        }

        if (err.status == 404) {
          this.toastr.error('Tài khoản không tồn tại!');
        }
      }
    );
  }
}
