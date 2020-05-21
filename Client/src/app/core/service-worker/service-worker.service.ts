import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ServiceWorkerService {

  constructor(private swUpdate: SwUpdate, private snackbar: MatSnackBar) {
    this.swUpdate.available.subscribe(evt => {
      const snack = this.snackbar.open('Update Available', 'Update');

      snack
        .onAction()
        .subscribe(() => {
          this.updateToLatest();
          window.location.reload();
        });
    });
  }


  public checkForUpdate(): void {
    if (this.swUpdate.isEnabled) {
      this.swUpdate.checkForUpdate().then(() => {
        console.log('Checking for updates...');
      }).catch((err) => {
        console.error('Error when checking for update', err);
      });
    }
  }

  private updateToLatest(): void {
    console.log('Updating to latest version.');
    this.swUpdate.activateUpdate().then(() => document.location.reload());
  }
}
