import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { ShortUrlsTableComponent } from './components/short-urls-table/short-urls-table';
import { ShortUrlInfoComponent } from './components/short-url-info/short-url-info';
import { authGuard } from './guards/auth.guard';


export const routes: Routes = [
    { path: '', component: ShortUrlsTableComponent },
    { path: 'login', component: LoginComponent },
    { path: 'urls/:id', component: ShortUrlInfoComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: '' }
];
