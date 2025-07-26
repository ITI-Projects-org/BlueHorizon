import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Review } from './Components/review/review';
import { EmailConfirmation } from './Pages/email-confirmation/email-confirmation';
import { Units } from './Pages/units/units';
import { CreateQr } from './Components/create-qr/create-qr';
import { GoogleSignup } from './Pages/google-signup/google-signup';
import { GoogleLoginSuccess } from './Pages/google-login-success/google-login-success';
import { GoogleSignupFail } from './Pages/google-signup-fail/google-signup-fail';
import { GoogleLoginFail } from './Pages/google-login-fail/google-login-fail';
import { Chat } from './Components/chat/chat';
import { Profile } from './Pages/profile/profile';
import { ResetPassword } from './Pages/reset-password/reset-password';
import { ChangePassword } from './Pages/change-password/change-password';
import { AddUnit } from './Components/add-unit/add-unit';
import { AuthGuard } from './Guards/auth-guard-guard';
import { BookingList } from './Pages/booking-list/booking-list';
import { BookingForm } from './Pages/booking-form/booking-form';
import { UnitDetailsComponent } from './Components/unit-details/unit-details';

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
  { path: 'VerifyOwner', component: OwnerVerification },
  { path: 'PendingOwners', component: PendingOwners },
  { path: 'profile', component: Profile },
  { path: 'reset-password', component: ResetPassword },
  { path: 'change-password', component: ChangePassword },
  { path: 'my-bookings', component: BookingList },
  { path: 'add-booking', component: BookingForm },

  {
    path: 'chat',
    component: Chat,
    canActivate: [AuthGuard],
  },
  // 4. Catch-all route: Redirects any unmatched URLs to the login page.
  //    This ensures users don't land on a blank page if they type a wrong URL.
  { path: 'addReview', component: Review, canActivate: [AuthGuard] },
  { path: 'createQr', component: CreateQr },
  { path: 'units', component: Units },
  { path: 'addunit', component: AddUnit },
  { path: 'unitDetails/:id', component: UnitDetailsComponent },

  { path: '**', redirectTo: 'login' },
];
