import { Component, OnInit } from '@angular/core';
import { AuthService } from "../auth.service";
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})

export class ForgotPasswordComponent implements OnInit {
  form = new FormGroup({
    email: new FormControl('', Validators.required),
  });

  get email() { return this.form.get('email'); }
  
  constructor(private authService: AuthService) { }

  ngOnInit() { }


  resetPassword() {
    // this.authService.ForgotPassword(passwordResetEmail.value)
  }

}