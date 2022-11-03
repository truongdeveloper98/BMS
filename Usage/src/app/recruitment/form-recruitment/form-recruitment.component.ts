import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  FrameworkVM,
  LanguageVM,
  LevelVM,
} from 'src/app/core/model/setting.model';
import { Positions } from 'src/app/core/model/project';
import { RecruitmentViewModel } from 'src/app/core/model/recruitment.model';
import { SettingService } from 'src/app/core/services/setting.service';
import { PositionService } from 'src/app/core/services/position.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateFrameworkComponent } from 'src/app/setting/create-framework/create-framework.component';
// import * as _moment from 'moment';
// import { default as _rollupMoment, Moment } from 'moment';
// const moment = _rollupMoment || _moment;

@Component({
  selector: 'app-form-recruitment',
  templateUrl: './form-recruitment.component.html',
  styleUrls: ['./form-recruitment.component.scss'],
})
export class FormRecruitmentComponent implements OnInit {
  form!: FormGroup;
  levels!: LevelVM[];
  languages!: LanguageVM[];
  frames!: FrameworkVM[];
  positions!: Positions[];
  @Output()
  onSaveChanges = new EventEmitter<any>();

  @Input()
  models!: RecruitmentViewModel;
  otherFramework!: number;
  otherPosition!: number;

  constructor(
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private projectService: PositionService,
    private settingService: SettingService
  ) {}
  ngOnInit(): void {
    var today = new Date();
    var defaultDate = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate(),
      0,
      0,
      0
    );

    this.projectService.getPositions().subscribe((res) => {
      this.positions = res;
    });
    this.settingService.getLevels().subscribe((res) => {
      this.levels = res;
    });
    this.settingService.getFrameworks().subscribe((res) => {
      this.frames = res;
    });
    this.settingService.getLanguages().subscribe((res) => {
      this.languages = res;
    });
    this.form = this.formBuilder.group({
      salarymin: ['', { validators: Validators.required }],
      salarymax: ['', { validators: Validators.required }],
      quantity: ['', { validators: Validators.required }],
      description: [''],
      datePublish: [defaultDate, { validators: Validators.required }],
      dateOnBroad: ['', { validators: Validators.required }],
      positionId: ['', { validators: Validators.required }],
      frameworkId: ['', { validators: Validators.required }],
      languageId: ['', { validators: Validators.required }],
      levelId: ['', { validators: Validators.required }],
    });

    if (this.models !== undefined) {
      this.form.patchValue({
        salarymin: this.models.SalaryMin,
        salarymax: this.models.SalaryMax,
        quantity: this.models.Quantity,
        description: this.models.Description,
        datePublish: this.models.DatePublish,
        dateOnBroad: this.models.DateOnBroad,
        positionId: this.models.PositionId,
        frameworkId: this.models.FrameworkId,
        levelId: this.models.LevelId,
        languageId: this.models.LanguageId,
      });
    }

    this.form.get('frameworkId')?.valueChanges.subscribe((value) => {
      this.otherFramework = value;
      if (this.otherFramework === 6) {
        this.openFormCreate(1);
      }
    });
    this.form.get('positionId')?.valueChanges.subscribe((value) => {
      this.otherPosition = value;
      if (this.otherPosition === 6) {
        this.openFormCreate(0);
      }
    });
  }
  reset() {
    this.form.patchValue({
      salarymin: '',
      salarymax: '',
      quantity: '',
      description: '',
      datePublish: '',
      dateOnBroad: '',
      positionIds: '',
      frameworkIds: '',
      levelIds: '',
      languageIds: '',
    });
  }
  saveChanges() {
    this.onSaveChanges.emit(this.form.value);
  }
  openFormCreate(other : number) {
    const dialogRef = this.dialog.open(CreateFrameworkComponent, {
      disableClose: true,
      width: '850px',
      maxHeight: '97vh',
      data: {other},
    });
    dialogRef.afterClosed().subscribe(() => {
      this.settingService.getFrameworks().subscribe((res) => {
        this.frames = res;
      });
    });
  }
}
