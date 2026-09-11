import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  constructor(public authService: AuthService) { }

  ngOnInit(): void {
    this.authService.loadCurrentUser().subscribe({
      error: () => { }
    });
  }

  onLogout(): void {
    this.authService.logout().subscribe();
  }
}