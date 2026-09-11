import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ShortUrlService } from '../../services/short-url.service';
import { ShortUrl } from '../../models/short-url.model';

@Component({
  selector: 'app-short-url-info',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './short-url-info.html',
  styleUrl: './short-url-info.scss'
})
export class ShortUrlInfoComponent implements OnInit {
  url = signal<ShortUrl | null>(null);
  notFound = signal(false);

  constructor(
    private route: ActivatedRoute,
    private shortUrlService: ShortUrlService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.shortUrlService.getById(id).subscribe({
      next: (url) => this.url.set(url),
      error: () => this.notFound.set(true)
    });
  }
}