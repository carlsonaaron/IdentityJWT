import { Component, OnInit } from '@angular/core';
import { AuthService } from './core/auth/auth.service';
import { ServiceWorkerService } from './core/service-worker/service-worker.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  sideMenuOpen: boolean;

  constructor(public authService: AuthService, private serviceWorkerService: ServiceWorkerService) { }

  ngOnInit() {
    this.serviceWorkerService.checkForUpdate();
  }
}
