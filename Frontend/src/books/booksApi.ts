import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type { BookSummary, GetBooksRequest } from "./types";

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