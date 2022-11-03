import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'dateVN' })
export class DateVNPipe implements PipeTransform {

  transform(value: Date): string {
    if (!value) {
      return '';
    }

    var format = 'dd-MM-yyyy';
    var datePipe = new DatePipe('vi-VN');

    var rs = datePipe.transform(new Date(value), format);

    return rs ? rs : '';
  }

}
