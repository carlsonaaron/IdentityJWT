import { Component, OnInit } from '@angular/core';
import { HomeService } from './home.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  anonymousResult: string;
  adminResult: string;
  userResult: string;

  userRoles: any[];
  adminRoles: any[];
  allRoles: any[];

  constructor(private homeService: HomeService) { }

  ngOnInit() {
    this.homeService.getAdminRole().subscribe(res => {
      this.adminRoles = res;
    });
    this.homeService.getUserRole().subscribe(res => {
      this.userRoles = res;
    });
    this.homeService.getAllRoles().subscribe(res => {
      this.allRoles = res;
    });
  }

}
