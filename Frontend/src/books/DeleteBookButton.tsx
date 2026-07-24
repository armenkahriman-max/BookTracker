import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentMember } from "../auth/useCurrentMember";
import { deleteBook } from "./booksApi";

type DeleteBookButtonProps = {
  bookId: number;
  title: string;
};

export function DeleteBookButton({ bookId, title }: DeleteBookButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const currentMemberQuery = useCurrentMember();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  function leaveDeletedBook() {
    queryClient.invalidateQueries({
      queryKey: ["books"],
      refetchType: "none",
    });
    queryClient.removeQueries({
      queryKey: ["books", "detail", bookId],
      exact: true,
    });
    navigate("/books");
  }

  const deleteMutation = useMutation({
    mutationFn: () => deleteBook(bookId),
    onSuccess: leaveDeletedBook,
  });

   if (
    !currentMemberQuery.isSuccess ||
    currentMemberQuery.data.role !== "Administrator"
  ) {
    return null;
  }
   if (!confirming) {
    return (
      <button type="button" onClick={() => setConfirming(true)}>
        Delete book
      </button>
    );
  }

    const mutationStatus =
    deleteMutation.error instanceof ApiError
      ? deleteMutation.error.status
      : null;

  return (
    <section aria-labelledby="delete-book-heading">
      <h2 id="delete-book-heading">Delete {title}?</h2>
      <p>This action cannot be undone.</p>

      <button
        type="button"
        onClick={() => deleteMutation.mutate()}
        disabled={deleteMutation.isPending}
      >
        {deleteMutation.isPending ? "Deleting..." : "Yes, delete book"}
      </button>{" "}

      <button
        type="button"
        onClick={() => {
          deleteMutation.reset();
          setConfirming(false);
        }}
        disabled={deleteMutation.isPending}
      >
        Cancel
      </button>

      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 403 && (
        <p>Only administrators can delete books.</p>
      )}
      {mutationStatus === 404 && (
        <div>
          <p>This book no longer exists. It may already have been deleted.</p>
          <button type="button" onClick={leaveDeletedBook}>
            Back to books
          </button>
        </div>
      )}
      {deleteMutation.isError && mutationStatus === null && (
        <p>Could not delete the book.</p>
      )}
    </section>
  );
  
}