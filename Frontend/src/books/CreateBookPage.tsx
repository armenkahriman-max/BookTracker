import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { createBook } from "./BooksApi";

export function CreateBookPage() {
  const [formError, setFormError] = useState<string | null>(null);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const createMutation = useMutation({
    mutationFn: createBook,
    onSuccess: async (book) => {
      await queryClient.invalidateQueries({ queryKey: ["books"] });
      navigate(`/books/${book.id}`);
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const title = formData.get("title")?.toString().trim() ?? "";
    const author = formData.get("author")?.toString().trim() ?? "";
    const yearValue = formData.get("year")?.toString().trim() ?? "";
    const year = Number(yearValue);

    if (!title || !author || !yearValue || !Number.isInteger(year)) {
      setFormError("Enter a title, author and valid year.");
      return;
    }

    createMutation.mutate({ title, author, year });
  }

  const unauthorized =
    createMutation.error instanceof ApiError &&
    createMutation.error.status === 401;

  const forbidden =
    createMutation.error instanceof ApiError &&
    createMutation.error.status === 403;

  const badRequest =
    createMutation.error instanceof ApiError &&
    createMutation.error.status === 400;

  return (
    <main>
      <Link to="/books">Cancel</Link>
      <h1>Add book</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Title
          <input name="title" maxLength={100} required />
        </label>

        <label>
          Author
          <input name="author" maxLength={100} required />
        </label>

        <label>
          Year
          <input name="year" type="number" required />
        </label>

        <button type="submit" disabled={createMutation.isPending}>
          {createMutation.isPending ? "Saving..." : "Add book"}
        </button>
      </form>

      {formError && <p>{formError}</p>}
      {badRequest && <p>The API rejected the book data.</p>}
      {unauthorized && <p>Your login is missing or expired.</p>}
      {forbidden && <p>Only administrators can add books.</p>}
      {createMutation.isError &&
        !badRequest &&
        !unauthorized &&
        !forbidden && <p>Could not add the book.</p>}
    </main>
  );
}