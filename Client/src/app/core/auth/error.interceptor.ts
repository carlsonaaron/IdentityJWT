import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (request.url.includes("Account/Refresh")) {
                // auto logout if error occurs while refreshing auth token using RefreshToken
                this.authService.Logout();
                location.reload(true);
            }

            const error = ((err.error && err.error.message) || err.statusText);
            return throwError(error);
        }))
    }
}