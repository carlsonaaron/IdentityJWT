import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { DomSanitizer } from '@angular/platform-browser';
import { MatIconRegistry } from '@angular/material/icon';

@Component({
  selector: 'app-main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent {
  @ViewChild('drawer', { static: false }) sidenav: any;
  private isHandset: boolean;
  fileData: File = null;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => {
        this.isHandset = result.matches;
        return this.isHandset;
      }),
      shareReplay()
    );



  constructor(
    private breakpointObserver: BreakpointObserver,
    public authService: AuthService,
    iconRegistry: MatIconRegistry,
    sanitizer: DomSanitizer
  ) {
    iconRegistry.addSvgIcon('sign-out', sanitizer.bypassSecurityTrustResourceUrl('assets/images/exit_to_app-24px.svg'));
  }


  closeSideNav() {
    if (this.isHandset) {
      this.sidenav.close();
    }
  }

}
