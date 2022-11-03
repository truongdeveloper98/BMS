import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PartnerModel } from 'src/app/core/model/partner.model';
import { PartnerService } from 'src/app/core/services/partner.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-create-partner',
  templateUrl: './create-partner.component.html',
  styleUrls: ['./create-partner.component.scss'],
})
export class CreatePartnerComponent implements OnInit {
  createPartner: any;
  checkpart = 0;
  checkcus = 1;
  selectCreatePartners = [
    {
      name: 'Partner',
      value: 0,
    },

    {
      name: 'Customer',
      value: 1,
    },
  ];
  form!: FormGroup;
  project: any;
  constructor(
    private dialogRef: MatDialogRef<CreatePartnerComponent>,
    private formBuilder: FormBuilder,
    private partnerService: PartnerService,
    private toasr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.project = data.report;
  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      projecttype: this.project,
    });
    this.createPartner = this.form.get('projecttype')?.value;
    this.form.get('projecttype')?.valueChanges.subscribe((value) => {
      this.createPartner = value;
    });
  }

  saveChanges(partner: PartnerModel) {
    if (this.createPartner === 0) {
      partner.IsPartner = true;
      partner.IsCustomer = false;
    } else {
      partner.IsPartner = false;
      partner.IsCustomer = true;
    }
    this.partnerService.CreatePartner(partner).subscribe(
      () => {
        if (this.createPartner === 0) {
          this.toasr.open('Thêm mới partner thành công!');
        } else {
          this.toasr.open('Thêm mới customer thành công!');
        }
        this.CloseDialog();
      },
      (err: any) => {
        if (this.createPartner === 0) {
          this.toasr.open('Thêm mới partner thất bại!');
        } else {
          this.toasr.open('Thêm mới customer thất bại!');
        }
        this.CloseDialog();
      }
    );
  }

  CloseDialog() {
    this.dialogRef.close();
  }
}
