import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { JwtHelperService } from '@auth0/angular-jwt';
import { PartnerModel } from 'src/app/core/model/partner.model';
import { ProjectTypes, Users } from 'src/app/core/model/project';
import { PartnerService } from 'src/app/core/services/partner.service';
import { ProjectTypeService } from 'src/app/core/services/project-type.service';
import { ProjectService } from 'src/app/core/services/project.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  pjid: any;
  users!: Users[];
  projectTypes!: ProjectTypes[];

  projectId: string | null | undefined;
  fontStyle?: string;
  createPartner: any;
  partner!: PartnerModel[];
  customer!: PartnerModel[];
  isDisabled: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    public service: ProjectService,
    public partnerService: PartnerService,
    public dialogRef: MatDialogRef<ProjectComponent>,
    private projectTypeService: ProjectTypeService,
    private projectService: ProjectService,
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data.projectId) {
      this.projectId = data.projectId;
      this.service.getProject(this.projectId);
    }
    this.projectTypeService.getProjectTypes().subscribe(
      (res) => {
        this.projectTypes = res;
      },
      (err) => {
        console.log(err);
      }
    );
  }

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
    if (event.keyCode === 13) {
      this.onSubmit();
    }
  }
  selectCreatePartners = [
    {
      name: 'Normal',
      value: 0,
    },

    {
      name: 'Partner',
      value: 1,
    },
  ];
  form!: FormGroup;
  ngOnInit(): void {
    this.form = this.formBuilder.group({
      projecttype: 0,
    });
    this.createPartner = this.form.get('projecttype')?.value;
    this.form.get('projecttype')?.valueChanges.subscribe((value) => {
      this.createPartner = value;
    });

    const helper = new JwtHelperService();
          const decodedToken = helper.decodeToken(
            localStorage.getItem('token')!
          );
    var roleName =
      decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];      
    if (roleName === 'Manager' || roleName === 'SysAdmin') {
      this.isDisabled=false;
    }

    this.projectService.getUsers().subscribe(
      (res) => {
        this.users = res;
      },
      (err) => {
        console.log(err);
      }
    );

    this.partnerService.getPartnerForProject(0).subscribe(
      (res) => {
        this.partner = res;
      },
      (err) => {
        console.log(err);
      }
    );

    this.partnerService.getPartnerForProject(1).subscribe(
      (res) => {
        this.customer = res;
      },
      (err) => {
        console.log(err);
      }
    );
  }

  onSubmit() {
    if (this.service.form.valid) {
      //new project
      if (!this.service.form.get('id')?.value) {
        this.service.insertProject(this.service.form.value).subscribe(
          (result) => {
            this.toastr.open('Thêm mới dự án thành công!');

            this.service.form.reset();
            this.service.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Thêm mới dự án thất bại!');
            console.log(error);
          }
        );
      }
      //update project
      else {
        this.service.setEnable();
        this.service.updateProject(this.service.form.value).subscribe(
          (result) => {
            this.toastr.open('Cập nhật dự án thành công!');

            this.service.form.reset();
            this.service.initializeFormGroup();
            this.onClose();
          },
          (error) => {
            this.toastr.error('Cập nhật dự án thất bại!');
          }
        );
      }
    }
  }

  onClear() {
    this.service.form.reset();
    this.service.initializeFormGroup();
  }

  onClose() {
    this.service.form.reset();
    this.service.initializeFormGroup();
    this.dialogRef.close();
  }
}
