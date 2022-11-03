import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Subject } from 'rxjs';
import { AppUserClaim } from '../core/model/app-user-claim';
import { HomeService } from '../core/services/home.service';
import { environment } from './../../environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  siderbarOpen : any;
  eventsSubject: Subject<void> = new Subject<void>();
  
  constructor() { }

  ngOnInit(): void {

  }

  siderbarToggler($event: any){
    this.siderbarOpen = $event;
  }

  toggleSidebarMenuLeft(){
    this.eventsSubject.next();
  }
}
