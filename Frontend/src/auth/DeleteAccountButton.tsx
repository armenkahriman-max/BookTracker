import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { ApiError } from "../api";
import { removeAccessToken } from "./tokenStorage"; 
import { useCurrentMember } from "./useCurrentMember"; 
import { useDeleteMember } from "../members/MemberHooks";
import { authKeys, memberKeys } from "../members/MemberKeys"; 

export function DeleteAccountButton() {
  const [isConfirming, setIsConfirming] = useState(false);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const currentMemberQuery = useCurrentMember();
  const deleteMutation = useDeleteMember();
  
  if (!currentMemberQuery.data) {
    return null;
  } 

  const member = currentMemberQuery.data;

  
  function handleDelete() {
    deleteMutation.mutate(member.id, {
      onSuccess: async () => {
        
        removeAccessToken(); 

        
        queryClient.removeQueries({ queryKey: authKeys.me });

       
        queryClient.invalidateQueries({ queryKey: memberKeys.lists() });

       
        navigate("/login", { replace: true });
      },
    });
  }

  
  if (isConfirming) {
    return (
      <div role="alert" style={{ border: "1px solid red", padding: "1rem", marginTop: "1rem" }}>
        <p>
          <strong>Are you sure you want to delete your account?</strong>
        </p>
        <p>
          Account: <strong>{member.name}</strong> ({member.email})
        </p>
        <p>This action <strong>cannot be undone</strong>.</p>

        <button
          type="button"
          onClick={handleDelete}
          disabled={deleteMutation.isPending}
        >
          {deleteMutation.isPending ? "Deleting..." : "Yes, delete my account"}
        </button>

        <button
          type="button"
          onClick={() => setIsConfirming(false)}
          disabled={deleteMutation.isPending}
          style={{ marginLeft: "0.5rem" }}
        >
          Cancel
        </button>

      
        {deleteMutation.isError && (
          <p role="alert" style={{ color: "red" }}>
            {deleteMutation.error instanceof ApiError
              ? `Error ${deleteMutation.error.status}: Could not delete account.`
              : "Could not delete account."}
          </p>
        )}
      </div>
    );
  }

  
  return (
    <button
      type="button"
      onClick={() => setIsConfirming(true)}
      style={{ marginTop: "1rem", color: "red" }}
    >
      Delete my account
    </button>
  );
}