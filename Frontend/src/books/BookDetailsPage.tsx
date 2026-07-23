import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getBook } from "./booksApi";

function readBookId(value: string | undefined) {
  const bookId = Number(value);
  return Number.isInteger(bookId) && bookId > 0 ? bookId : null;
}

export function BookDetailsPage() {
  const { bookId: bookIdParameter } = useParams();
  const bookId = readBookId(bookIdParameter);

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

  if (bookId === null) {
    return (
      <main>
        <h1>Invalid book id</h1>
        <Link to="/books">Back to books</Link>
      </main>
    );
  }

  if (bookQuery.isPending) {
    return <p>Loading book...</p>;
  }

  const notFound =
    bookQuery.error instanceof ApiError && bookQuery.error.status === 404;

  if (notFound) {
    return (
      <main>
        <h1>Book not found</h1>
        <p>The requested book does not exist.</p>
        <Link to="/books">Back to books</Link>
      </main>
    );
  }

  if (bookQuery.isError) {
    return (
      <main>
        <h1>Could not load the book</h1>
        <p>Is the API running?</p>
        <Link to="/books">Back to books</Link>
      </main>
    );
  }

  const book = bookQuery.data;

  return (
    <main>
      <Link to="/books">Back to books</Link>
      <h1>{book.title}</h1>
      <p>Author: {book.author}</p>
      <p>Year: {book.year}</p>
    </main>
  );
}