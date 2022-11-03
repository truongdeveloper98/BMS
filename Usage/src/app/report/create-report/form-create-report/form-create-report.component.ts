import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  CreateReportVm,
  PositionVm,
  ProjectVm,
} from 'src/app/core/model/report.model';

@Component({
  selector: 'app-form-create-report',
  templateUrl: './form-create-report.component.html',
  styleUrls: ['./form-create-report.component.scss'],
})
export class FormCreateReportComponent implements OnInit {
  inputHour: string = '';
  constructor(private formBuilder: FormBuilder) { }
  form!: FormGroup;
  @Output()
  onSaveChanges = new EventEmitter<any>();
  @Input()
  model!: CreateReportVm;
  @Input() positions: PositionVm[] = [];
  @Input() projects: ProjectVm[] = [];
  @Input()
  reportUpdate: any;
  @Input()
  reportType: any;
  @Input()
  reset!: boolean;
  mindate!: Date;
  maxdate!: Date;

  @Input() isSaveChange!: boolean;
  rate = [
    {
      name: '100%',
      value: 100,
    },

    {
      name: '120%',
      value: 120,
    },
    {
      name: '130%',
      value: 130,
    },
    {
      name: '150%',
      value: 150,
    },
  ];
  workingtypes = [
    { name: 'Offline', value: 2 },
    { name: 'Remote', value: 0 },
    { name: 'Onsite', value: 3 },
  ];

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 13) {
      this.saveChanges();
    }
  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      thuhai: [true],
      thuba: [true],
      thutu: [true],
      thunam: [true],
      thusau: [true],
      startdate: [
        '',
        {
          validators: Validators.required,
        },
      ],

      enddate: [
        '',
        {
          validators: Validators.required,
        },
      ],
      workinghour: [
        '',
        {
          validators: Validators.required,
        },
      ],
      note: [
        '',
        {
          validators: Validators.required,
        },
      ],
      ratevalue: [
        0,
        {
          validators: Validators.required,
        },
      ],
      projectid: [
        '',
        {
          validators: Validators.required,
        },
      ],
      positionid: [
        '',
        {
          validators: Validators.required,
        },
      ],
      workingtype: [
        2,
        {
          validators: Validators.required,
        },
      ],
    });
    this.form.get('startdate')?.valueChanges.subscribe((value) => {
      this.mindate = value;
    });
    this.form.get('enddate')?.valueChanges.subscribe((value) => {
      this.maxdate = value;

    });
  }
  saveChanges() {
    this.onSaveChanges.emit(this.form.value);

    if (this.isSaveChange) {
      this.form.setValue({
        thuhai: true,
        thuba: true,
        thutu: true,
        thunam: true,
        thusau: true,
        workinghour: 0,
        startdate: '',
        enddate: '',
        note: ' ',
        positionid: 0,
        projectid: 0,
        ratevalue: 0,
        workingtype: 0,
      });
    }
  }

  checkInputTime(form: any) {
    var s: string = this.form.get('workinghour')?.value;
    var n: number = +s;
    if (s !== '') {
      if (!isNaN(n) && n > 0 && n <= 8) {
        this.inputHour = s;
      }
      else {
        this.form.get('workinghour')?.patchValue(this.inputHour);
      }
    }
    else this.inputHour = '';

  }
}
