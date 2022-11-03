import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PartnerModel } from 'src/app/core/model/partner.model';
import { PartnerService } from 'src/app/core/services/partner.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-edit-partner',
  templateUrl: './edit-partner.component.html',
  styleUrls: ['./edit-partner.component.scss'],
})
export class EditPartnerComponent implements OnInit {
  id!: number;
  CheckCus!: number;
  constructor(
    private partnerService: PartnerService,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<EditPartnerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.id = data.Id;
    this.CheckCus = data.Check;
  }

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 27) {
      this.dialogRef.close();
    }
  }
  update = true;
  models!: PartnerModel;
  ngOnInit(): void {
    this.partnerService.getById(this.id).subscribe((values) => {
      this.models = values;
    });
  }

  saveChanges(partner: PartnerModel) {
    partner.PartnerId = this.id;
    this.partnerService.edit(partner).subscribe(
      () => {
        this.toastr.open('Cập nhập thành công!');
        this.CloseDialog();
      },
      (err: any) => {
        this.toastr.open('Cập nhập thất bại!');
        this.CloseDialog();
      }
    );
  }

  CloseDialog() {
    this.dialogRef.close();
  }
}
