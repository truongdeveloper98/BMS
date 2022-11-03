import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';
import { userCreationDTO, userVM } from 'src/app/core/model/userService.model';
import moment from 'moment';

@Component({
  selector: 'app-edit-users',
  templateUrl: './edit-users.component.html',
  styleUrls: ['./edit-users.component.scss'],
})
export class EditUsersComponent implements OnInit {
  id: any;
  Departments: any;
  constructor(
    private userService: UsersService,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<EditUsersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.id = data.Id;
    this.Departments = data.listDepartment;
  }

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
  }

  update = true;
  models: any;
  roles: any;
  types: any;
  ngOnInit(): void {
    this.userService.getRole().subscribe((roles) => {
      this.roles = roles;
    });
    this.userService.getById(this.id).subscribe((res) => {
      this.models = res;
    });

    this.userService.getTypeList().subscribe((types) => {
      this.types = types;
    });
  }

  saveChanges(formData: userCreationDTO) {
    formData.id = this.id;
    let model: userVM = {
      id: this.id,
      username: formData.username,
      email: formData.email,
      first_name: formData.first_name,
      last_name: formData.last_name,
      birth_date: formData.birth_date,
      start_date: formData.start_date,
      end_date: formData.end_date,
      address: formData.address,
      avatar: formData.avatar,
      password: '',
      phone: formData.phone,
      role: formData.role,
      typeid: formData.type,
      info: {
        userid: this.id,
        level: formData.level,
        team: formData.team,
        position: formData.position,
        totalleaveday: formData.totalleaveday,
        occupiedleaveday: formData.occupiedleaveday,
        department: formData.department,
        ispending: formData.ispending,
        pendingstart: formData.pendingstart,
        pendingend: formData.pendingend,
        avaiableleaveday: 0,
        typeid: formData.type,
        effortfree: formData.effortfree,
        company: formData.company,
        cvlink: formData.cvlink,
      },
    };
    console.log(model);
    this.userService.edit(model).subscribe(
      () => {
        this.toastr.open('Cập nhập người dùng thành công!');
        this.dialogRef.close();
      },
      (error) => {
        this.toastr.error('Cập nhập người dùng thất bại!');
      }
    );
  }
}
