import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';

import { AuthGuard } from './guards/auth-guard';
import { Chat } from './components/chat/chat';


export const routes: Routes = [
  {path: '', redirectTo: 'login', pathMatch: 'full'},
  { path: 'login', component: LoginComponent },
  {
    path: 'chat',
    component: Chat,
    canActivate: [AuthGuard]
  },
  // 4. Catch-all route: Redirects any unmatched URLs to the login page.
  //    This ensures users don't land on a blank page if they type a wrong URL.
  { path: '**', redirectTo: 'login' }
];
