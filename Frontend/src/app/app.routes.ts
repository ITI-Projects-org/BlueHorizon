import { Routes } from '@angular/router';
import { Units } from './Pages/units/units';

export const routes: Routes = [
    { path: '', redirectTo: 'units', pathMatch: 'full' },
    { path: 'units', component: Units },
];
