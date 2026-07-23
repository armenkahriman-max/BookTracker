import { keepPreviousData, useQuery } from "@tanstack/react-query";
import type { FormEvent } from "react";
import { useSearchParams } from "react-router-dom";
import { getBooks } from "./booksApi";

const pageSize = 10;

function readPage(value: string | null) {
  const page = Number(value);
  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function BookListPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = readPage(searchParams.get("page"));
  const search = searchParams.get("search")?.trim() ?? "";

  const booksQuery = useQuery({
    queryKey: ["books", { page, pageSize, search }],
    queryFn: () => getBooks({ page, pageSize, search }),
    placeholderData: keepPreviousData,
  });

  function setPage(nextPage: number) {
    const next = new URLSearchParams(searchParams);

    if (nextPage === 1) {
      next.delete("page");
    } else {
      next.set("page", nextPage.toString());
    }

    setSearchParams(next);
  }

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const next = new URLSearchParams();
    const formData = new FormData(event.currentTarget);
    const value = formData.get("search")?.toString().trim() ?? "";

    if (value) {
      next.set("search", value);
    }

    setSearchParams(next);
  }

    if (booksQuery.isPending) {
    return <p>Loading books...</p>;
  }

  if (booksQuery.isError) {
    return <p>Could not load the books. Is the API running?</p>;
  }

  const result = booksQuery.data;

  return (
    <main>
      <h1>Books</h1>

      <form key={search} onSubmit={handleSearch}>
        <label>
          Search by title or author
          <input
            type="search"
            name="search"
            defaultValue={search}
          />
        </label>
        <button type="submit">Search</button>
      </form>

      {result.items.length === 0 ? (
        <p>No books found.</p>
      ) : (
        <ul>
          {result.items.map((book) => (
            <li key={book.id}>
              <strong>{book.title}</strong> by {book.author}
            </li>
          ))}
        </ul>
      )}

      <p>
        Page {result.page} of {result.totalPages}. {result.totalItems} books found.
      </p>

      <button
        type="button"
        onClick={() => setPage(result.page - 1)}
        disabled={result.page <= 1 || booksQuery.isFetching}
      >
        Previous
      </button>{" "}

      <button
        type="button"
        onClick={() => setPage(result.page + 1)}
        disabled={result.page >= result.totalPages || booksQuery.isFetching}
      >
        Next
      </button>

      {booksQuery.isFetching && <p>Updating books...</p>}
    </main>
  );
}