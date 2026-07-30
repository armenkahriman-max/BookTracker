import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ApiError } from "../api";
import { getMember, updateMember } from "./MembersApi";
import { memberKeys } from "./MemberKeys";
import type { UpdateMemberRequest } from "./types";

function readMemberId(value: string | undefined) {
    const memberId = Number(value);
    return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function EditMemberPage() {
    const { memberId: memberIdParameter } = useParams();
    const memberId = readMemberId(memberIdParameter);
    const [formError, setFormError] = useState<string | null>(null);
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const memberQuery = useQuery({
        queryKey: memberKeys.detail(memberId!),
        queryFn: () => {
            if (memberId === null) {
                throw new Error("Invalid member id");
            }
            return getMember(memberId);
        },
        enabled: memberId !== null,
        retry: false,
    });

    const updateMutation = useMutation({
        mutationFn: (request: UpdateMemberRequest) => {
            if (memberId === null) {
                throw new Error("Invalid member id");
            }
            return updateMember(memberId, request);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: memberKeys.all});
            navigate(`/members/${memberId}`);
        },
    });

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    if (!memberQuery.data) {
        return;
    }
    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email ) {
        setFormError("Enter a name and email.");
        return;
    }
    updateMutation.mutate({
        name,
        email,
    });

  }

  if (memberId === null) {
    return (
        <main>
            <h1>Edit Member</h1>
            <p>Invalid member id.</p>
            <Link to="/members">Back to members</Link>
        </main>
    );
  }
    
  if (memberQuery.isLoading) {
    return (
        <main>
            <h1>Edit Member</h1>
            <p>Loading...</p>
        </main>
    );
  }

  if (memberQuery.isError || !memberQuery.data) {
    const message =
    memberQuery.error instanceof ApiError
    ? memberQuery.error.message
    : "Failed to load member.";
    return (
        <main>
            <h1>Edit Member</h1>
            <p role="alert">{message}</p>
            <button type="button" onClick={() => memberQuery.refetch()}>
                Retry
            </button>
            <Link to="/members">Back to members</Link>
        </main>
    );
  }
    const member = memberQuery.data;

    const mutationStatus =
    updateMutation.error instanceof ApiError
    ? updateMutation.error.status
    : null;

    return (
        <main>
            <Link to={`/members/${member.id}`}>Cancel</Link>
            <h1>Edit {member.name}</h1>

            <form onSubmit={handleSubmit}>
                <label>
                    Name
                    <input
                    name = "name"
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
                    type= "email"
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
                    <p role="alert">You are not allowed to edit this member.</p>
                )}
                {mutationStatus === 404 && (
                    <p role="alert">This member no longer exists</p>
                )}
                {mutationStatus === 409 && (
                    <p role="alert">This email address is already in use by another member.</p>
                )}
    {updateMutation.isError && mutationStatus === null && (
        <p role="alert">Could not update member.</p>
    )}
        </main>


    );
  }
