import { useState, type FormEvent } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { getAccessToken } from "./tokenStorage";
import { useCurrentMember } from "./useCurrentMember";
import { useUpdateMember } from "../members/MemberHooks";
import type { UpdateMemberRequest } from "../members/types";

export function EditAccountPage() {
  const [formError, setFormError] = useState<string | null>(null);
  const navigate = useNavigate();
  const currentMemberQuery = useCurrentMember();
  const updateMutation = useUpdateMember();

  
  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

 

  if (currentMemberQuery.isPending) {
    return (
      <main>
        <h1>Edit Account</h1>
        <p>Loading…</p>
      </main>
    );
  }

 
  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  
  if (currentMemberQuery.isError || !currentMemberQuery.data) {
    return (
      <main>
        <h1>Edit Account</h1>
        <p role="alert">Could not load your account.</p>
        <Link to="/account">Back to account</Link>
      </main>
    );
  }

  const member = currentMemberQuery.data;

 
  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email) {
      setFormError("Name and email are required.");
      return;
    }

   
    if (!email.includes("@")) {
      setFormError("Please enter a valid email address.");
      return;
    }

    const request: UpdateMemberRequest = { name, email };

    updateMutation.mutate(
      { memberId: member.id, request },
      {
        onSuccess: () => {
          navigate("/account");
        },
      },
    );
  }

 
  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main>
      <Link to="/account">Cancel</Link>
      <h1>Edit Account</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Name
          <input
            name="name"
            defaultValue={member.name}
            maxLength={100}
            required
            disabled={updateMutation.isPending}
          />
        </label>

        <label>
          Email
          <input
            name="email"
            type="email"
            defaultValue={member.email}
            maxLength={100}
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
        <p role="alert">The name or email is invalid.</p>
      )}
      {mutationStatus === 401 && (
        <p role="alert">Your login is missing or expired.</p>
      )}
      {mutationStatus === 403 && (
        <p role="alert">You are not allowed to edit this account.</p>
      )}
      {mutationStatus === 404 && (
        <p role="alert">Account not found.</p>
      )}
      {mutationStatus === 409 && (
        <p role="alert">
          This email address is already in use by another member.
        </p>
      )}
      {updateMutation.isError && mutationStatus === null && (
        <p role="alert">Could not update the account.</p>
      )}
    </main>
  );
}