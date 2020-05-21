import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { DashboardComponent } from './dashboard/dashboard.component';

import { AuthService } from './core/auth/auth.service';
import { AuthGuard } from './core/auth/auth.guard';
import { SignInComponent } from './core/auth/sign-in/sign-in.component';
import { ForgotPasswordComponent } from './core/auth/forgot-password/forgot-password.component';
import { MainNavComponent } from './core/main-nav/main-nav.component';
import { LayoutModule } from '@angular/cdk/layout';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtInterceptor } from './core/auth/jwt.interceptor';
import { ErrorInterceptor } from './core/auth/error.interceptor';
import { ServiceWorkerService } from './core/service-worker/service-worker.service';
import { HomeComponent } from './home/home.component';
import { HomeService } from './home/home.service';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    SignInComponent,
    ForgotPasswordComponent,
    MainNavComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
    SharedModule,
    LayoutModule,
    HttpClientModule,
    BrowserAnimationsModule,
  ],
  providers: [
    AuthService,
    AuthGuard,
    ServiceWorkerService,
    HomeService,

    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
