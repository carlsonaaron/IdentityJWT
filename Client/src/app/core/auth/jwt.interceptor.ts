import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { AuthService } from './auth.service';
import { catchError } from 'rxjs/internal/operators/catchError';
import { switchMap } from 'rxjs/internal/operators/switchMap';
import { filter } from 'rxjs/internal/operators/filter';
import { take } from 'rxjs/internal/operators/take';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    private refreshTokenInProgress = false;
    private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

    constructor(private authService: AuthService) {
        this.refreshTokenSubject.subscribe(rt => console.log(rt));
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // if (request.url.indexOf('Account/Refresh') !== -1 || request.url.indexOf('Account/Login') !== -1) {
        if (request.url.indexOf('Account/Refresh') >= 0) {
            return next.handle(request);
        }

        const accessExpired = this.authService.isAccessTokenExpired();
        const refreshExpired = !this.authService.getRefreshToken();  // TODO: Add RefreshToken expirations

        if (accessExpired && refreshExpired) {
            return next.handle(request);
        }

        if (accessExpired && !refreshExpired) {
            if (!this.refreshTokenInProgress) {
                this.refreshTokenInProgress = true;
                this.refreshTokenSubject.next(null);
                return this.authService.Refresh().pipe(
                    switchMap((authResponse) => {
                        if (authResponse) {
                            this.refreshTokenInProgress = false;
                            this.refreshTokenSubject.next(authResponse.refreshToken);
                            return next.handle(this.injectToken(request));
                        }
                    }),
                );
            } else {
                return this.refreshTokenSubject.pipe(
                    filter(result => result !== null),
                    take(1),
                    switchMap((res) => {
                        return next.handle(this.injectToken(request))
                    })
                );
            }
        }

        if (!accessExpired) {
            return next.handle(this.injectToken(request));
        }
    }

    injectToken(request: HttpRequest<any>) {
        const accessToken = this.authService.getAccessToken();
        return request.clone({
            setHeaders: {
                Authorization: `Bearer ${accessToken}`
            }
        });
    }
}