import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type { BookDetails, BookSummary, GetBooksRequest, CreateBookRequest, CreateBookResponse } from "./types";
 

export function createBook(request: CreateBookRequest) {
  return apiRequest<CreateBookResponse>("/books", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
export function getBook(bookId: number) {
  return apiRequest<BookDetails>(`/books/${bookId}`);
}
export function getBooks(request: GetBooksRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<BookSummary>>(
    `/books?${parameters.toString()}`,
  );
}

