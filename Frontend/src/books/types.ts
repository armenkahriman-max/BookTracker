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