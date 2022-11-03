import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PartnerModel } from 'src/app/core/model/partner.model';

@Component({
  selector: 'app-form-partner',
  templateUrl: './form-partner.component.html',
  styleUrls: ['./form-partner.component.scss'],
})
export class FormPartnerComponent implements OnInit {
  constructor(private formBuilder: FormBuilder) {}
  hide = true;
  form!: FormGroup;
  tabSelected = 0;
  @Output()
  onSaveChanges = new EventEmitter<any>();
  @Input()
  models!: PartnerModel;
  @Input()
  CheckCus!: number;

  maxRatingArr = [5];
  selectedRate = 0;
  previousRate = 0;

  @HostListener('keyup', ['$event'])
  onkeyup(event: any) {
    if (event.keyCode === 13) {
      this.saveChanges();
    }
  }
  ngOnInit(): void {
    this.maxRatingArr = Array(5).fill(0);
    this.form = this.formBuilder.group({
      PartnerName: ['', { validators: Validators.required }],
      Address: [''],
      Website: [''],
      Note: [''],
      Vote: [''],
    });
    if (this.models !== undefined) {
      this.form.patchValue({
        PartnerName: this.models.PartnerName,
        Address: this.models.Address,
        Website: this.models.Website,
        Note: this.models.Note,
      });
      this.selectedRate = this.models.Vote;
    }
  }

  handeleMouseEnter(index: number) {
    this.selectedRate = index + 1;
  }
  Rate(index: number) {
    this.selectedRate = index + 1;
    this.previousRate = this.selectedRate;
  }
  handleMouseLeave() {
    if (this.previousRate !== 0) {
      this.selectedRate = this.previousRate;
    } else {
      this.selectedRate = 0;
    }
  }

  saveChanges() {
    this.form.patchValue({
      Vote: this.selectedRate,
    });
    this.onSaveChanges.emit(this.form.value);
  }
}
