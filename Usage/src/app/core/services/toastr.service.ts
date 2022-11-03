import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarComponent } from 'src/app/shared/layout/snack-bar/snack-bar.component';

@Injectable({
  providedIn: 'root'
})
export class ToastrService {

  //private snackBar!: MatSnackBar;

  constructor(private snackBar: MatSnackBar) { }

  public open(message: string, action?: string) {
    this.snackBar.openFromComponent(SnackBarComponent, {
      duration: 2000,
      panelClass: ['snackbar-success'],
      verticalPosition: 'top',
      horizontalPosition: 'right',
      data: { message: message, action: action },
    });
  }

  public error(message: string, action?: string) {
    this.snackBar.openFromComponent(SnackBarComponent, {
      duration: 5000,
      panelClass: ['snackbar-error'],
      verticalPosition: 'top',
      horizontalPosition: 'right',
      data: { message: message, action: action },
    });
  }

  public info(message: string, action?: string) {
    this.snackBar.openFromComponent(SnackBarComponent, {
      duration: 2000,
      panelClass: ['snackbar-info'],
      verticalPosition: 'top',
      horizontalPosition: 'right',
      data: { message: message, action: action },
    });
  }

  public warning(message: string, action?: string) {
    this.snackBar.openFromComponent(SnackBarComponent, {
      duration: 2000,
      panelClass: ['snackbar-warning'],
      verticalPosition: 'top',
      horizontalPosition: 'right',
      data: { message: message, action: action },
    });
  }
  
}
