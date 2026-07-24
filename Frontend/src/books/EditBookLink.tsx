import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditBookLinkProps = {
  bookId: number;
};

export function EditBookLink({ bookId }: EditBookLinkProps) {
  const currentMemberQuery = useCurrentMember();

  if (
    !currentMemberQuery.isSuccess ||
    currentMemberQuery.data.role !== "Administrator"
  ) {
    return null;
  }

  return <Link to={`/books/${bookId}/edit`}>Edit book</Link>;
}