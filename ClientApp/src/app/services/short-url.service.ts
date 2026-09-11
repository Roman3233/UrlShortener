import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ShortUrl, CreateShortUrlRequest } from '../models/short-url.model';

const API_URL = 'http://localhost:5062/api/shorturls';

@Injectable({ providedIn: 'root' })
export class ShortUrlService {
    constructor(private http: HttpClient) { }

    getAll(): Observable<ShortUrl[]> {
        return this.http.get<ShortUrl[]>(API_URL);
    }

    getById(id: number): Observable<ShortUrl> {
        return this.http.get<ShortUrl>(`${API_URL}/${id}`);
    }

    create(request: CreateShortUrlRequest): Observable<ShortUrl> {
        return this.http.post<ShortUrl>(API_URL, request);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_URL}/${id}`);
    }
}