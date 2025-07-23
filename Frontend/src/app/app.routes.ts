import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Review } from './Components/review/review';
import { EmailConfirmation } from './Pages/email-confirmation/email-confirmation';
import { AuthGuard } from './guards/auth-guard';
import { Chat } from './components/chat/chat';
export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'email-confirmed', component: EmailConfirmation },
  { path: 'VerifyOwner', component: OwnerVerification },
  { path: 'PendingOwners', component: PendingOwners },
    { path: 'addReview', component: Review },
    {
        path: 'chat',
        component: Chat,
        canActivate: [AuthGuard]
    },
    // 4. Catch-all route: Redirects any unmatched URLs to the login page.
    //    This ensures users don't land on a blank page if they type a wrong URL.
    { path: '**', redirectTo: 'login' }
];
