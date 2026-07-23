import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { getCurrentMember } from "./authApi";
import { getAccessToken, removeAccessToken } from "./tokenStorage";

export function AccountPage() {
  const currentMemberQuery = useQuery({
    queryKey: ["current-member"],
    queryFn: getCurrentMember,
    enabled: getAccessToken() !== null,
    retry: false,
  });

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  useEffect(() => {
    if (unauthorized) {
      removeAccessToken();
    }
  }, [unauthorized]);

  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isPending) {
    return <p>Loading account...</p>;
  }

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isError) {
    return <p>Could not load the account.</p>;
  }

  const member = currentMemberQuery.data;

  return (
    <main>
      <h1>{member.name}</h1>
      <p>{member.email}</p>
      <p>Role: {member.role}</p>
    </main>
  );
}