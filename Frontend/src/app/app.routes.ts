import { QrCodeDto } from './Models/qr-code-dto';
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

import { AuthGuard } from './guards/auth-guard';
import { Chat } from './components/chat/chat';
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
    {
        path: 'chat',
        component: Chat,
        canActivate: [AuthGuard]
    },
    // 4. Catch-all route: Redirects any unmatched URLs to the login page.
    //    This ensures users don't land on a blank page if they type a wrong URL.
    { path: '**', redirectTo: 'login' }
  { path: 'addReview', component: Review },
  { path: 'createQr', component:  CreateQr},
    { path: '', component: Units },
];
