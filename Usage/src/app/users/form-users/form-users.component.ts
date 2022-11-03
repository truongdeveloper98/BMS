import {
  AfterViewInit,
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Positions } from 'src/app/core/model/project';
import { CompanyViewModel } from 'src/app/core/model/userService.model';
import { PositionService } from 'src/app/core/services/position.service';
import { UsersService } from 'src/app/core/services/user.service';
import { FormControl } from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { MatSelect } from '@angular/material/select';

@Component({
  selector: 'app-form-users',
  templateUrl: './form-users.component.html',
  styleUrls: ['./form-users.component.scss'],
})
export class FormUsersComponent implements OnInit, OnDestroy {
  constructor(
    private formBuilder: FormBuilder,
    public positionSerivce: PositionService,
    private usersService: UsersService
  ) {}
  hide = true;
  form!: FormGroup;
  partner: any;
  tabSelected = 0;
  @Output()
  onSaveChanges = new EventEmitter<any>();
  @Input()
  models!: any;
  positions!: Positions[];
  @Input() roles: any;
  @Input() types: any;
  @Input() departments: any;
  @Input()
  update: any;
  mindate!: Date;
  maxdate!: Date;
  minpendingdate!: Date;
  maxpendingdate!: Date;
  monthOffAvailable: number = 0;
  tabHeight: any;
  levels: any = [
    'Fresher',
    'Junior',
    'Middle Junior',
    'Senior',
    'Tech Lead',
    'Manager',
  ];
  teams: any = [
    'Java',
    'PHP',
    '.NET',
    'Mobile',
    'Front end',
    'Tester',
    'Comtor & BrSE',
    'Other',
  ];
  companies!: CompanyViewModel[];

  /** control for the MatSelect filter keyword */
  public companyFilterCtrl: FormControl = new FormControl();

  /** list of banks filtered by search keyword */
  public filteredCompanies: ReplaySubject<CompanyViewModel[]> = new ReplaySubject<CompanyViewModel[]>(1);

  protected _onDestroy = new Subject<void>();

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 13) {
      this.saveChanges();
    }
  }

  ngOnInit(): void {
    this.positionSerivce.getPositions().subscribe((res) => {
      this.positions = res;
    });
    this.usersService.getCompanies().subscribe((res) => {
      this.companies = res;
      // load the initial bank list
    this.filteredCompanies.next(this.companies.slice());
    });  

    if (this.update === false) {
      this.tabHeight = '320px';
      this.form = this.formBuilder.group({
        username: ['', { validators: Validators.required }],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]],
        birth_date: [null],
        start_date: ['', { validators: Validators.required }],
        end_date: null,
        avatar: null,
        role: ['', { validators: Validators.required }],
        position: ['', { validators: Validators.required }],
        type: ['', { validators: Validators.required }],
        first_name: ['', { validators: Validators.required }],
        last_name: ['', { validators: Validators.required }],
        address: [''],
        department: ['', { validators: Validators.required }],
        phone: [''],
        totalleaveday: '0',
        occupiedleaveday: '0',
        team: ['', { validators: Validators.required }],
        level: ['', { validators: Validators.required }],
        ispending: [false],
        pendingstart: '',
        effortfree: '100',
        company: 0,
        cvlink: '',
      });
    } else {
      this.tabHeight = '230px';
      this.form = this.formBuilder.group({
        username: [''],
        email: [''],
        password: [''],
        birth_date: [null],
        start_date: ['', { validators: Validators.required }],
        end_date: null,
        avatar: null,
        role: ['', { validators: Validators.required }],
        position: ['', { validators: Validators.required }],
        type: ['', { validators: Validators.required }],
        first_name: ['', { validators: Validators.required }],
        last_name: ['', { validators: Validators.required }],
        address: [''],
        department: ['', { validators: Validators.required }],
        phone: [''],
        totalleaveday: '',
        occupiedleaveday: '',
        team: ['', { validators: Validators.required }],
        level: ['', { validators: Validators.required }],
        ispending: [false],
        pendingstart: null,
        effortfree: '100',
        company: 0,
        cvlink: '',
      });
    }

    this.form.get('start_date')?.valueChanges.subscribe((value) => {
      this.mindate = value;
    });
    this.form.get('end_date')?.valueChanges.subscribe((value) => {
      this.maxdate = value;
    });
    this.form.get('pendingstart')?.valueChanges.subscribe((value) => {
      this.minpendingdate = value;
    });
    this.form.get('pendingend')?.valueChanges.subscribe((value) => {
      this.maxpendingdate = value;
    });

    
    // listen for search field value changes
     this.companyFilterCtrl.valueChanges
       .pipe(takeUntil(this._onDestroy))
       .subscribe(() => {
        this.filterCompanies();
    }); 
    if (this.models !== undefined) {
      this.form.patchValue({
        first_name: this.models.First_Name,
        last_name: this.models.Last_Name,
        birth_date: this.models.Birth_Date,
        email: this.models.Email,
        username: this.models.UserName,
        start_date: this.models.Start_Date,
        end_date: this.models.End_Date,
        avatar: this.models.Avatar,
        address: this.models.Address,
        phone: this.models.PhoneNumber,
        role: this.models.Role,
        position: this.models.Info.Position,
        department: this.models.Info.Department,
        id: this.models.Id,
        type: this.models.Info.TypeId,
        totalleaveday: this.models.Info.TotalLeaveDay,
        occupiedleaveday: this.models.Info.OccupiedLeaveDay,
        level: this.models.Info.Level,
        team: this.models.Info.Team,
        ispending: this.models.Info.IsPending,
        company: this.models.Info.Company,
        cvlink: this.models.Info.CVLink,
      });
      if (this.models.Info.TypeId !== 3)
        this.form.patchValue({
          totalleaveday: 0,
          occupiedleaveday: 0,
        });
      if (this.models.Info.IsPending) {
        this.form.patchValue({
          effortfree: this.models.Info.EffortFree,
        });
        if (this.models.Info.PendingStart)
          this.form.patchValue({
            pendingstart: this.models.Info.PendingStart,
          });
        if (this.models.Info.PendingEnd)
          this.form.patchValue({
            pendingend: this.models.Info.PendingEnd,
          });
      }
    }
  } 

  onPendingChange() {
    let pending: boolean = this.form.get('ispending')?.value;
    if (pending) {
      this.form.get('pendingstart')?.addValidators(Validators.required);
      this.form.get('effortfree')?.addValidators(Validators.required);
    } else {
      this.form.get('pendingstart')?.setErrors(null);
      this.form.get('pendingstart')?.clearValidators();
      this.form.get('pendingstart')?.updateValueAndValidity();

      this.form.get('effortfree')?.setErrors(null);
      this.form.get('effortfree')?.clearValidators();
      this.form.get('effortfree')?.updateValueAndValidity();
    }
  }
  saveChanges() {
    if (this.form.controls['start_date'].valid) {
      this.onSaveChanges.emit(this.form.value);
    }
  }

  protected filterCompanies() {
    if (!this.companies) {
      return;
    }
    // get the search keyword
    let search = this.companyFilterCtrl.value;
    if (!search) {
      this.filteredCompanies.next(this.companies.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the banks
    this.filteredCompanies.next(
      this.companies.filter(company => company.CompanyName.toLowerCase().indexOf(search) > -1)
    );
  }

  ngOnDestroy() {
     this._onDestroy.next();
     this._onDestroy.complete();
  }
}
