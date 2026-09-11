
export interface User {
    id: number;
    login: string;
    role: 'Admin' | 'User';
}

export interface LoginRequest {
    login: string;
    password: string;
}