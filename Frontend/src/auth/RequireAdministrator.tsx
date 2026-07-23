import { Navigate, Outlet } from "react-router-dom";
import { getAccessToken } from "./tokenStorage";
import { useCurrentMember } from "./useCurrentMember";

export function RequireAdministrator() {
  const currentMemberQuery = useCurrentMember();

  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isPending) {
    return <p>Checking permissions...</p>;
  }

  if (currentMemberQuery.isError) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.data.role !== "Administrator") {
    return (
      <main>
        <h1>Forbidden</h1>
        <p>Only administrators can manage books.</p>
      </main>
    );
  }

  return <Outlet />;
}