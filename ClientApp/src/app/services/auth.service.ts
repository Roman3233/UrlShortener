import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { User, LoginRequest } from '../models/user.model';

const API_URL = 'http://localhost:5062/api/auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
    currentUser = signal<User | null>(null);

    constructor(private http: HttpClient) { }

    login(request: LoginRequest): Observable<User> {
        return this.http.post<User>(`${API_URL}/login`, request).pipe(
            tap(user => this.currentUser.set(user))
        );
    }

    logout(): Observable<void> {
        return this.http.post<void>(`${API_URL}/logout`, {}).pipe(
            tap(() => this.currentUser.set(null))
        );
    }

    loadCurrentUser(): Observable<User> {
        return this.http.get<User>(`${API_URL}/me`).pipe(
            tap(user => this.currentUser.set(user))
        );
    }

    isAuthenticated(): boolean {
        return this.currentUser() !== null;
    }

    isAdmin(): boolean {
        return this.currentUser()?.role === 'Admin';
    }
}