import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  constructor() { }

  public getLocation(): Promise<Coordinates> {
    let options = { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 };

    return new Promise((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(result => {
        // On Geolocation Success...
        if (result && result.coords) {
          resolve(result.coords);
        }      
      }, () => { 
        resolve(null);
      } , options);
    })
  }
}
