import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  title: string = 'Confirm';
  message: string = 'Are you sure?';
  message2: string = 'message';
  request: string = '';
  labelOK: string = 'Yes';
  labelCancel: string = 'No';
  secondDialog: boolean = false;
  dialogIndex: number = 1;
  response: string = '';
  inputtype: any = 'text';
  dateresponse: any;
  textresponse: any;
  dateForm!: FormGroup;
  constructor(
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.title = data.title;
    this.message = data.message;
    this.labelOK = data.labelOK;
    this.labelCancel = data.labelCancel;
    if (data.secondDialog) this.secondDialog = data.secondDialog;
    if (data.dialogIndex) this.dialogIndex = data.dialogIndex;
    if (data.message2) this.message2 = data.message2;
    if (data.request) this.request = data.request;
    if (data.inputtype) this.inputtype = data.inputtype;
  }

  //press ESC to close dialog
  @HostListener('window:keyup.esc') onKeyUp() {
    this.dialogRef.close(false);
  }

  ngOnInit(): void {
    if(this.inputtype === 'date'){
      this.dateForm = this.formBuilder.group({
        date: ['', { validators: Validators.required }]
      })
    }
    
  }

  onOK() {
    this.dialogRef.close(true);
  }

  onCancel() {
    if (this.secondDialog) {
      this.dialogIndex++
      if (this.dialogIndex !== 2) this.dialogRef.close(false);
    }
    else this.dialogRef.close(false);
  }

  onReject() {
    this.dialogRef.close(this.getResponse());
  }

  onClose() {
    this.dialogRef.close(false);
  }

  getResponse(): any{
    switch(this.inputtype){
      case 'text':
        return this.textresponse
        break
      case 'date':
        return this.dateresponse
        break

    }
  }

  validateInput(){
    if(this.inputtype === 'date') return this.dateForm.invalid
    return false
  }
}
