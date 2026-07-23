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