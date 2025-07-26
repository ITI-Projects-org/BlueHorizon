// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { EmailConfirmation } from './Pages/email-confirmation/email-confirmation';
import { GoogleSignup } from './Pages/google-signup/google-signup';
import { GoogleSignupFail } from './Pages/google-signup-fail/google-signup-fail';
import { GoogleLoginSuccess } from './Pages/google-login-success/google-login-success';
import { GoogleLoginFail } from './Pages/google-login-fail/google-login-fail';
import { ResetPassword } from './Pages/reset-password/reset-password';
import { ChangePassword } from './Pages/change-password/change-password';
import { Units } from './Pages/units/units';
import { UnitDetailsComponent } from './Components/unit-details/unit-details';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Profile } from './Pages/profile/profile';
import { BookingList } from './Pages/booking-list/booking-list';
import { BookingForm } from './Pages/booking-form/booking-form';
import { AddUnit } from './Components/add-unit/add-unit';
import { ChatComponent } from './Components/chat/chat';
import { Review } from './Components/review/review';
import { CreateQr } from './Components/create-qr/create-qr';
import { AuthGuard } from './Guards/auth-guard-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'email-confirmed', component: EmailConfirmation },
  { path: 'google-signup', component: GoogleSignup },
  { path: 'google-signup-fail', component: GoogleSignupFail },
  { path: 'google-login-success', component: GoogleLoginSuccess },
  { path: 'google-login-fail', component: GoogleLoginFail },
  { path: 'reset-password', component: ResetPassword },
  { path: 'change-password', component: ChangePassword },
  { path: 'units', component: Units },
  { path: 'unitDetails/:id', component: UnitDetailsComponent },
  { path: 'VerifyOwner', component: OwnerVerification, canActivate: [AuthGuard] },
  { path: 'PendingOwners', component: PendingOwners, canActivate: [AuthGuard] },
  { path: 'profile', component: Profile, canActivate: [AuthGuard] },
  { path: 'my-bookings', component: BookingList, canActivate: [AuthGuard] },
  { path: 'add-booking', component: BookingForm, canActivate: [AuthGuard] },
  { path: 'addunit', component: AddUnit, canActivate: [AuthGuard] },
  { path: 'chat', component: ChatComponent, canActivate: [AuthGuard] },
  { path: 'addReview', component: Review, canActivate: [AuthGuard] },
  { path: 'createQr', component: CreateQr, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'login' },
];
