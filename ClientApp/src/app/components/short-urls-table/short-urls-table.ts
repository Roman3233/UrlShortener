import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ShortUrlService } from '../../services/short-url.service';
import { AuthService } from '../../services/auth.service';
import { ShortUrl } from '../../models/short-url.model';

@Component({
  selector: 'app-short-urls-table',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './short-urls-table.html'
})
export class ShortUrlsTableComponent implements OnInit {
  urls = signal<ShortUrl[]>([]);
  newUrl = '';
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  constructor(
    private shortUrlService: ShortUrlService,
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadUrls();
  }

  loadUrls(): void {
    this.shortUrlService.getAll().subscribe({
      next: (urls) => this.urls.set(urls)
    });
  }

  onAddUrl(): void {
    if (!this.newUrl.trim()) {
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.shortUrlService.create({ originalUrl: this.newUrl }).subscribe({
      next: (created) => {
        this.urls.update(current => [created, ...current]);
        this.newUrl = '';
        this.isSubmitting.set(false);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        if (err.status === 409) {
          this.errorMessage.set('This URL has already been shortened.');
        } else {
          this.errorMessage.set('Something went wrong. Please try again.');
        }
      }
    });
  }

  onDelete(id: number): void {
    this.shortUrlService.delete(id).subscribe({
      next: () => {
        this.urls.update(current => current.filter(u => u.id !== id));
      }
    });
  }

  canDelete(url: ShortUrl): boolean {
    const user = this.authService.currentUser();
    if (!user) {
      return false;
    }
    return user.role === 'Admin' || user.id === url.createdByUserId;
  }
}