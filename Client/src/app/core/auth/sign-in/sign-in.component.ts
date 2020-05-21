import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs/internal/operators/first';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent implements OnInit {
  returnUrl: string;
  loading = false;
  submitted = false;
  error = '';

  // UI based on https://www.positronx.io/create-login-ui-template-with-angular-8-material-design/
  form = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    rememberMe: new FormControl(false)
  });

  get email() { return this.form.get('email'); }
  get password() { return this.form.get('password'); }
  get rememberMe() { return this.form.get('rememberMe')}

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    if (this.authService.currentUserValue) { 
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  login() {
    this.submitted = true;

    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this.authService.Login(this.email.value, this.password.value, this.rememberMe.value)
      .pipe(first())
      .subscribe(
        data => {
          this.router.navigate([this.returnUrl]);
        },
        error => {
          this.error = error;
          console.log({error: error});
          this.loading = false;
        }
      )
  }
}
