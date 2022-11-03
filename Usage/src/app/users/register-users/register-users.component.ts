import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostListener,
  Inject,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import moment from 'moment';
import { ToastrService } from 'src/app/core/services/toastr.service';
import { UsersService } from 'src/app/core/services/user.service';
import {
  RoleViewModel,
  userCreationDTO,
  userVM,
} from '../../core/model/userService.model';

@Component({
  selector: 'app-register-users',
  templateUrl: './register-users.component.html',
  styleUrls: ['./register-users.component.scss'],
})
export class RegisterUsersComponent implements OnInit {
  Departments: any;
  constructor(
    private user: UsersService,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<RegisterUsersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.Departments = data.listDepartment;
  }

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
  }

  update = false;
  model: any;
  roles: any;
  types: any;
  ngOnInit(): void {
    this.user.getRole().subscribe((role) => {
      this.roles = role;
    });

    this.user.getTypeList().subscribe((types) => {
      this.types = types;
    });
  }

  saveChanges(userCreate: userCreationDTO) {
    let model: userVM = {
      id: '',
      username: userCreate.username,
      email: userCreate.email,
      first_name: userCreate.first_name,
      last_name: userCreate.last_name,
      birth_date: userCreate.birth_date,
      start_date: userCreate.start_date,
      end_date: new Date(
        moment(userCreate.end_date).utcOffset(420).format('YYYY-MM-DD')
      ),
      address: userCreate.address,
      avatar: userCreate.avatar,
      password: userCreate.password,
      phone: userCreate.phone,
      role: userCreate.role,
      typeid: userCreate.type,
      info: {
        userid: '',
        level: userCreate.level,
        position: userCreate.position,
        team: userCreate.team,
        totalleaveday: userCreate.totalleaveday,
        occupiedleaveday: userCreate.occupiedleaveday,
        department: userCreate.department,
        ispending: userCreate.ispending,
        pendingstart: userCreate.pendingstart,
        pendingend: userCreate.pendingend,
        avaiableleaveday: 0,
        typeid: userCreate.type,
        effortfree: userCreate.effortfree,
        company: userCreate.company,
        cvlink: userCreate.cvlink,
      },
    };
    this.user.CreateUser(model).subscribe((result: any) => {
      if (result.Succeeded === true) {
        this.toastr.open('Thêm mới người dùng thành công');
        this.dialogRef.close();
      } else {
        this.toastr.error('Error: ' + result.Errors[0].Code);
      }
    });
  }
}
