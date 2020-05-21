import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private http: HttpClient) { }

  getAllRoles() {
    const url = `${environment.webapiRoot}/Role`;
    return this.http.get<any[]>(url);
  }
  
  getAdminRole() {
    const url = `${environment.webapiRoot}/Role/AppAdmin`;
    return this.http.get<any[]>(url);
  }

  getUserRole() {
    const url = `${environment.webapiRoot}/Role/AppUser`;
    return this.http.get<any[]>(url);
  }
}
