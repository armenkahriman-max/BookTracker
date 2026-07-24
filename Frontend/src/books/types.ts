export type BookSummary = {
  id: number;
  title: string;
  author: string;
};

export type GetBooksRequest = {
  page: number;
  pageSize: number;
  search: string;
};

export type BookDetails = {
  id: number;
  title: string;
  author: string;
  year: number;
  version: string;
};

export interface CreateBookRequest {
  title: string;
  author: string;
  year: number;
}

export interface CreateBookResponse {
  id: number;
  title: string;
  author: string;
  year: number;
}

export type UpdateBookRequest = {
  title: string;
  author: string;
  year: number;
  version: string;
};