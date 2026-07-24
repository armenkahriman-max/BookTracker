import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getBook, updateBook } from "./booksApi";
import type { UpdateBookRequest } from "./types";

function readBookId(value: string | undefined) {
  const bookId = Number(value);
  return Number.isInteger(bookId) && bookId > 0 ? bookId : null;
}

export function EditBookPage() {
  const { bookId: bookIdParameter } = useParams();
  const bookId = readBookId(bookIdParameter);
  const [formError, setFormError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const bookQuery = useQuery({
    queryKey: ["books", "detail", bookId],
    queryFn: () => {
      if (bookId === null) {
        throw new Error("Invalid book id");
      }
      return getBook(bookId);
    },
    enabled: bookId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateBookRequest) => {
      if (bookId === null) {
        throw new Error("Invalid book id");
      }
      return updateBook(bookId, request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["books"] });
      navigate(`/books/${bookId}`);
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    if (!bookQuery.data) {
      return;
    }

    const formData = new FormData(event.currentTarget);
    const title = formData.get("title")?.toString().trim() ?? "";
    const author = formData.get("author")?.toString().trim() ?? "";
    const yearValue = formData.get("year")?.toString().trim() ?? "";
    const year = Number(yearValue);

    if (!title || !author || !yearValue || !Number.isInteger(year)) {
      setFormError("Enter a title, author and valid year.");
      return;
    }

    updateMutation.mutate({
      title,
      author,
      year,
      version: bookQuery.data.version,
    });
  }

  async function reloadLatest() {
    updateMutation.reset();
    await bookQuery.refetch();
  }

 

  if (bookId === null) {
    return (
      <main>
        <h1>Edit Book</h1>
        <p>Invalid book id.</p>
        <Link to="/books">Back to books</Link>
      </main>
    );
  }

  if (bookQuery.isLoading) {
    return (
      <main>
        <h1>Edit Book</h1>
        <p>Loading…</p>
      </main>
    );
  }

  if (bookQuery.isError || !bookQuery.data) {
    const message =
      bookQuery.error instanceof ApiError
        ? bookQuery.error.message
        : "Failed to load book.";
    return (
      <main>
        <h1>Edit Book</h1>
        <p role="alert">{message}</p>
        <button type="button" onClick={() => bookQuery.refetch()}>
          Retry
        </button>
        <Link to="/books">Back to books</Link>
      </main>
    );
  }

  const book = bookQuery.data;

  
  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main>
      <Link to={`/books/${book.id}`}>Cancel</Link>
      <h1>Edit {book.title}</h1>

      <form key={book.version} onSubmit={handleSubmit}>
        <label>
          Title
          <input
            name="title"
            defaultValue={book.title}
            maxLength={100}
            required
            disabled={updateMutation.isPending}
          />
        </label>

        <label>
          Author
          <input
            name="author"
            defaultValue={book.author}
            maxLength={100}
            required
            disabled={updateMutation.isPending}
          />
        </label>

        <label>
          Year
          <input
            name="year"
            type="number"
            defaultValue={book.year}
            required
            disabled={updateMutation.isPending}
          />
        </label>

        <button type="submit" disabled={updateMutation.isPending}>
          {updateMutation.isPending ? "Saving..." : "Save changes"}
        </button>
      </form>

      {formError && <p role="alert">{formError}</p>}

      {mutationStatus === 400 && (
        <p role="alert">The API rejected the book data.</p>
      )}
      {mutationStatus === 401 && (
        <p role="alert">Your login is missing or expired.</p>
      )}
      {mutationStatus === 403 && (
        <p role="alert">Only administrators can edit books.</p>
      )}
      {mutationStatus === 404 && (
        <p role="alert">This book no longer exists.</p>
      )}
      {mutationStatus === 409 && (
        <div role="alert">
          <p>
            This book was changed by another user. Your changes were not saved.
          </p>
          <button type="button" onClick={reloadLatest}>
            Load latest version
          </button>
        </div>
      )}
      {updateMutation.isError && mutationStatus === null && (
        <p role="alert">Could not update the book.</p>
      )}
    </main>
  );
}