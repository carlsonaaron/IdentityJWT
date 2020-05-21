import { Injectable, NgZone } from '@angular/core';
import { User } from "./user";
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LoginInfo } from './sign-in/login.model';
import { map } from 'rxjs/internal/operators/map';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Observable } from 'rxjs/internal/Observable';
import { Subject, EMPTY } from 'rxjs';
import { Router } from '@angular/router';
import * as jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUser$: Observable<User>;

  private signedOutSubject: Subject<void> = new Subject();
  public signedOut$ = this.signedOutSubject.asObservable();

  private JWT_TOKEN: string = 'jwt_token';
  private REFRESH_TOKEN: string = 'refresh_token';

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }
  
  public getAccessToken(): string {
    return localStorage.getItem(this.JWT_TOKEN);
  }

  public getRefreshToken(): string {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }


  // Returns true when user is logged in and email is verified
  get isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  // Sign in with email/password
  Login(email, password, rememberMe): Observable<User> {
    const url = `${environment.webapiRoot}/account/login`;
    const l: LoginInfo = {
      email: email,
      password: password,
      rememberMe: rememberMe
    };
    return this.http.post<any>(url, l)
      .pipe(map(response => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(response));
        this.setAccessToken(response.jwtToken);
        this.setRefreshToken(response.refreshToken)
        this.currentUserSubject.next(response);
        return response;
      }));
  }

  Refresh(): Observable<any> {
    const accessToken = localStorage.getItem(this.JWT_TOKEN);
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN);

    if (!refreshToken) {
      return EMPTY;
    }

    const url = `${environment.webapiRoot}/Account/Refresh`;
    const params = {
      accessToken: accessToken,
      refreshToken: refreshToken
    }

    return this.http.post<any>(url, params)
      .pipe(map(response => {
        this.setAccessToken(response.jwtToken);
        this.setRefreshToken(response.refreshToken);
        return response;
      }))
  }

  Logout() {
    const url = `${environment.webapiRoot}/Account/Logout`;

    localStorage.removeItem('currentUser');
    localStorage.removeItem(this.JWT_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
    
    this.currentUserSubject.next(null);

    this.signedOutSubject.next();
    this.http.post<void>(url, null)
      .toPromise()
      .then(() => {
        this.router.navigate(['login']);
      })
  }

  private setAccessToken(token: string) {
    localStorage.setItem(this.JWT_TOKEN, token);
  }

  private setRefreshToken(refreshToken: string) {
    localStorage.setItem(this.REFRESH_TOKEN, refreshToken);
  }

  private getTokenExpirationDate(token: string): Date {
    const decoded = jwt_decode(token);

    if (!decoded.exp) {
      return null;
    }

    const date = new Date(0);
    date.setUTCSeconds(decoded.exp);
    return date;
  }

  public isAccessTokenExpired(token?: string): boolean {
    if (!token) {
      token = this.getAccessToken();
      
      if (!token) {
        return true;
      }
    }    

    const date = this.getTokenExpirationDate(token);

    if (date === undefined) {
      return false;
    }

    return !(date.valueOf() > new Date().valueOf());
  }

}
