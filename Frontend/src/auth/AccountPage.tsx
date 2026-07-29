import { Link, Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { getAccessToken } from "./tokenStorage";
import { useCurrentMember } from "./useCurrentMember";
import { DeleteAccountButton } from "./DeleteAccountButton";

export function AccountPage() {
  
  const currentMemberQuery = useCurrentMember();

 
  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isPending) {
    return <p>Loading account...</p>;
  }

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isError || !currentMemberQuery.data) {
    return <p>Could not load the account.</p>;
  }

 
  const member = currentMemberQuery.data;

  return (
    <main>
      <h1>My Account</h1>

      <dl>
        <dt>Name</dt>
        <dd>{member.name}</dd>

        <dt>Email</dt>
        <dd>{member.email}</dd>

        <dt>Role</dt>
        <dd>{member.role}</dd>
      </dl>

      <p>
        <Link to="/account/edit">Edit account</Link>
      </p>

      
      <DeleteAccountButton />
    </main>
  );
}