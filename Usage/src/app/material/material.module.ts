import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatNativeDateModule  } from '@angular/material/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatChipsModule } from '@angular/material/chips';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import {
  NgxMatDatetimePickerModule,
  NgxMatTimepickerModule,
  NgxMatNativeDateModule,
  NgxMatDateFormats,
  NGX_MAT_DATE_FORMATS,
  NgxMatDateAdapter,
} from '@angular-material-components/datetime-picker';
import { 
  NgxMatMomentModule, 
  NgxMatMomentAdapter, 
  NGX_MAT_MOMENT_DATE_ADAPTER_OPTIONS 
} from '@angular-material-components/moment-adapter';


export const CUSTOM_MOMENT_FORMATS  = {
  parse: {
    dateInput: "DD/MM/YYYY, HH:mm"
  },
  display: {
    dateInput: "DD/MM/YYYY, HH:mm",
    monthYearLabel: "YYYY MMM",
    dateA11yLabel: "DD/MM/YYYY",
    monthYearA11yLabel: "YYYY MMM",
  }
};


@NgModule({
  declarations: [],
  exports: [
    NgxMatDatetimePickerModule,
    NgxMatNativeDateModule,
    NgxMatMomentModule,
    NgxMatTimepickerModule,
    MatMenuModule,
    MatSnackBarModule,
    MatToolbarModule,
    MatRadioModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatIconModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatInputModule,
    MatNativeDateModule,
    MatAutocompleteModule,
    MatTableModule,
    MatTabsModule,
    DragDropModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatDialogModule,
    MatPaginatorModule,
    MatChipsModule,
    ScrollingModule,
    MatSortModule,
    MatExpansionModule,
    MatButtonToggleModule,
  ],
  imports: [
    CommonModule
  ],
  providers: [
    { provide: NGX_MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: CUSTOM_MOMENT_FORMATS },
    { provide: NGX_MAT_DATE_FORMATS, useValue: CUSTOM_MOMENT_FORMATS },  
    { provide: NgxMatDateAdapter, useClass: NgxMatMomentAdapter },
  ],
})
export class MaterialModule {}
