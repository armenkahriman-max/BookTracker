export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  expiresAt: string;
};

export type CurrentMember = {
  id: number;
  name: string;
  email: string;
  role: string;
};

export type CreateBookRequest = {
  title: string;
  author: string;
  year: number;
};

export type CreateBookResponse = {
  id: number;
  title: string;
  author: string;
  year: number;
};