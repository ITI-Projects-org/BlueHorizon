<<<<<<< HEAD
import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Review } from './Components/review/review';

export const routes: Routes = [
 { path:"",redirectTo:"home", pathMatch:'full'},
 { path:"home",component:Home},
 { path:"login",component:Login },
 { path:"register",component:Register },
 { path:"VerifyOwner",component:OwnerVerification},
 { path:"PendingOwners",component:PendingOwners},
 { path:"addReview",component:Review}
=======
import { Routes } from '@angular/router';
import { Home } from './Pages/home/home';
import { Login } from './Pages/login/login';
import { Register } from './Pages/register/register';
import { OwnerVerification } from './Pages/Verification/owner-verification/owner-verification';
import { PendingOwners } from './Pages/Verification/pending-owners/pending-owners';
import { Review } from './Components/review/review';

export const routes: Routes = [
 { path:"",redirectTo:"home", pathMatch:'full'},
 { path:"home",component:Home},
 { path:"login",component:Login },
 { path:"register",component:Register },
 { path:"VerifyOwner",component:OwnerVerification},
 { path:"PendingOwners",component:PendingOwners},
 { path:"addReview",component:Review}
>>>>>>> cb72b14bdaef95076a0d8c8478c03b994869df8b
];