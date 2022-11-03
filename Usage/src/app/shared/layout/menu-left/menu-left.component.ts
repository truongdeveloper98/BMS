import { Component, ElementRef, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../confirm-dialog/confirm-dialog.component';
import { DocumentViewModel } from 'src/app/core/model/data';
import { HomeService } from 'src/app/core/services/home.service';
import { ToastrService } from 'src/app/core/services/toastr.service';

@Component({
  selector: 'app-menu-left',
  templateUrl: './menu-left.component.html',
  styleUrls: ['./menu-left.component.scss']
})
export class MenuLeftComponent implements OnInit {
  reportForbiddenList = ['Sale', 'HR']
  reportForbidden = false
  menuCount: number = 6;
  public collapseMenu: boolean[] = new Array(this.menuCount);
  roleName: string = '';
  isPm: number = 0;
  editEnableArr: boolean[] = [false, false];
  docList: DocumentViewModel[] = [{DocumentId: 1, Link: ''}, {DocumentId: 2, Link: ''}];
  @Input() childMessage!: boolean;
  @Output() toggleSidebarMenuLeft: EventEmitter<void> = new EventEmitter();

  constructor(
    private elem: ElementRef,
    public dialog: MatDialog,
    private homeService: HomeService,
    private toastr: ToastrService,
    ) { }

  ngOnInit(): void {
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(localStorage.getItem('token')!);

    this.roleName =
      decodedToken[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    if(this.reportForbiddenList.find(x => x === this.roleName)) this.reportForbidden = true

    this.isPm = +decodedToken[
      'isPm'
    ];

    this.homeService.getDocumentLink().subscribe((res) => {
      this.docList = res;
    });


  }

  menuLeftClose() {
    this.toggleSidebarMenuLeft.next();
  }

  collapseToggle(menu: number) {
    if (this.collapseMenu[menu] === false) this.collapseMenu[menu] = !this.collapseMenu[menu];
    else {
      for (var i = 0; i < this.menuCount; i++) {
        this.collapseMenu[i] = true;
      }
      this.collapseMenu[menu] = false;
    }


  }

  editDocumentLink(id: any){
    this.dialog
      .open(ConfirmDialogComponent, {
        disableClose: true,
        width: '550px',
        minHeight: '150px',
        data: {
          title: 'Documents',
          message2: 'Enter documents link:',
          labelCancel: 'Save',
          secondDialog: true,
          dialogIndex: 2,
          request: this.docList[id].Link,
        },
      })
      .afterClosed().subscribe((link) => {
        if(link !== false){
          const request: DocumentViewModel = {
            DocumentId: this.docList[id].DocumentId,
            Link:  link
          };
          this.homeService.setDocumentLink(request).subscribe(
            () => {
              this.toastr.open('Link updated!');
              this.homeService.getDocumentLink().subscribe((res) => {
                this.docList = res;
              });
            },
            () => {
              this.toastr.error('Failed to update link!');
            }
          );
        }
      });
  }

  editEnable(id: number, isEnable: boolean){
    if(this.roleName === 'Manager' || this.roleName === 'SysAdmin')
      this.editEnableArr[id] = isEnable;
  }

  openDocument(link: string){
    window.open(link, "_blank");
  }
}
