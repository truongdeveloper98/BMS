import { DatePipe } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';
import { ProfileViewModel } from 'src/app/core/model/userService.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  defaultProfileImage?: string;


  profileForm = this.formBuilder.group({
    Id: [''],
    Avatar: [''],
    First_Name: ['', Validators.required],
    Last_Name: ['', Validators.required],
    Email: [{value: '', disabled: true}, Validators.required],
    Birth_Date: [''],
    PhoneNumber: ['', [Validators.required,Validators.minLength(10)]],
    Address: [''],
  });

  constructor(
    private userService: UsersService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private datePipe: DatePipe
  ) {}

  @HostListener('keyup', ['$event'])
  onkeyup(event:any) {
    if(event.keyCode === 13){
      this.onSubmit();
    }
  }

  ngOnInit(): void {
    this.SetProfile();
  }

  onReset(){
    this.SetProfile();
  }

  SetProfile() {
    this.userService.getProfile().subscribe(
      (succ) => {
        this.profileForm.setValue({
          Id: succ.Id,
          Avatar: succ.Avatar,
          First_Name: succ.First_Name,
          Last_Name: succ.Last_Name,
          Email: succ.Email,
          Birth_Date: succ.Birth_Date,
          PhoneNumber: succ.PhoneNumber,
          Address: succ.Address,
        });
        this.setImage();
      },
      (err) => {
        this.setImage();
      }
    );
  }

  UploadImage(event: any): void {
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    var userId = decodedToken['UserID'];

    if (userId) {
      const file: File = event.target.files[0];
      this.userService.uploadImage(userId, file).subscribe(
        (suc) => {
          this.profileForm.patchValue({ Avatar: suc });

          this.toastr.open('Cập nhật Avatar thành công!');
          this.setImage();
        },
        (error) => {
          if (error.status == 400){
             this.toastr.error(error.error);
          }else if(error.status == 404){
            this.toastr.error(error.error);
          }else{
            this.toastr.error('Cập nhật Avatar thất bại!');
          }

          this.setImage();
        }
      );
    }
  }

  private setImage() {

    if (this.profileForm.controls['Avatar'].value) {
      this.defaultProfileImage = this.profileForm.controls['Avatar'].value

    } else {
      this.defaultProfileImage = '/assets/img/users.png';
    }
  }

  onSubmit() {
    if (this.profileForm.valid) {

      const request : ProfileViewModel ={
        First_Name: this.profileForm.controls['First_Name'].value,
        Last_Name : this.profileForm.controls['Last_Name'].value,
        Birth_Date: this.datePipe.transform(this.profileForm.controls['Birth_Date'].value, 'yyyy-MM-dd'),
        Address: this.profileForm.controls['Address'].value,
        PhoneNumber: this.profileForm.controls['PhoneNumber'].value
      }

       this.userService.updateProfile(request).subscribe(
        (res) => {
          this.toastr.open('Cập nhật thành công!');

          this.SetProfile();
        },
        (err) => {
          this.toastr.error('Cập nhật thất bại!');
        }
      );
    }
  }
}
