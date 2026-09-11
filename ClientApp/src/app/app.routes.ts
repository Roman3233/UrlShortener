import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { ShortUrlsTableComponent } from './components/short-urls-table/short-urls-table';


export const routes: Routes = [
    { path: '', component: ShortUrlsTableComponent },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: 'login' }
];
