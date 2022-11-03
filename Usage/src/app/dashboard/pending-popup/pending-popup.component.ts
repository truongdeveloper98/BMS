import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
@Component({
  selector: 'app-pending-popup',
  templateUrl: './pending-popup.component.html',
  styleUrls: ['./pending-popup.component.scss']
})
export class PendingPopupComponent implements OnInit {
  members: any
  title: any
  constructor(
    private dialogRef: MatDialogRef<PendingPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.members = data.detail
    this.title = data.title
   }
  //press ESC to close dialog
  @HostListener('window:keyup.esc') onKeyUp() {
    this.dialogRef.close(false);
  }

  ngOnInit(): void {
  }



  onClose() {
    this.dialogRef.close(false);
  }

}
