import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateVNPipe } from './date-vn.pipe';



@NgModule({
  declarations: [
    DateVNPipe
  ],
  imports: [
    CommonModule
  ],
  exports:[
    DateVNPipe
  ]
})
export class CustomPipeModule { }
