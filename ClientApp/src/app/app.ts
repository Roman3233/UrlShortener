import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './services/auth.service';
import { ToastComponent } from './components/toast/toast';
import { ToastService } from './services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  constructor(
    public authService: AuthService,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.authService.loadCurrentUser().subscribe({
      error: () => { }
    });
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.toastService.info('You have been logged out.')
    });
  }
}