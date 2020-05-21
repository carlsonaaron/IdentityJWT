import { DashboardComponent } from './dashboard/dashboard.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './core/auth/auth.guard';
import { SignInComponent } from './core/auth/sign-in/sign-in.component';
import { ForgotPasswordComponent } from './core/auth/forgot-password/forgot-password.component';
import { HomeComponent } from './home/home.component';


const routes: Routes = [
  { path: 'login', component: SignInComponent, data: { title: 'Login' }},
  { path: 'forgot-password', component: ForgotPasswordComponent, data: { title: 'Forgot Password' }},
  { path: 'dashboard', component: DashboardComponent, data: { title: 'Dashboard' }, canActivate: [AuthGuard] },
  { path: 'home', component: HomeComponent, data: { title: 'Home' }, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
