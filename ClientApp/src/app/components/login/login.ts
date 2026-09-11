import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  login = '';
  password = '';
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastService: ToastService
  ) { }

  onSubmit(): void {
    this.errorMessage.set(null);
    this.isLoading.set(true);

    this.authService.login({ login: this.login, password: this.password }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.toastService.success(`Welcome back, ${this.login}!`);
        this.router.navigate(['/']);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Invalid login or password.');
        this.toastService.error('Invalid login or password.');
      }
    });
  }
}