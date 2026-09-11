export interface ShortUrl {
    id: number;
    originalUrl: string;
    shortCode: string;
    shortUrl: string;
    createdDate: string;
    createdByUserId: number;
    createdByLogin: string;
}

export interface CreateShortUrlRequest {
    originalUrl: string;
}