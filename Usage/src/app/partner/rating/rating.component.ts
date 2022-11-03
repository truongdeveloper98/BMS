import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.css'],
})
export class RatingComponent implements OnInit {
  constructor() {}
  @Input()
  rateInit = 0;
  maxRatingArr = [this.rateInit];
  @Input()
  selectedRate = 3;

  previousRate = 0;
  ngOnInit(): void {
    this.maxRatingArr = Array(this.rateInit).fill(0);
  }

  Rate(index: number) {
    this.selectedRate = index + 1;
    this.previousRate = this.selectedRate;
  }
}
