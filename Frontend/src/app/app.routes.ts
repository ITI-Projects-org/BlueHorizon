import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Review } from './Components/review/review';
import { EmailConfirmation } from './Pages/email-confirmation/email-confirmation';
import { GoogleSignup } from './Pages/google-signup/google-signup';
import { GoogleLoginSuccess } from './Pages/google-login-success/google-login-success';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'email-confirmed', component: EmailConfirmation },
  { path: 'google-signup', component: GoogleSignup },
  { path: 'google-login-success', component: GoogleLoginSuccess },
  { path: 'VerifyOwner', component: OwnerVerification },
  { path: 'PendingOwners', component: PendingOwners },
  { path: 'addReview', component: Review },
];
