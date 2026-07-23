import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { ApiError } from "../api";
import { getCurrentMember } from "./authApi";
import { getAccessToken, removeAccessToken } from "./tokenStorage";

export function useCurrentMember() {
  const query = useQuery({
    queryKey: ["current-member"],
    queryFn: getCurrentMember,
    enabled: getAccessToken() !== null,
    retry: false,
  });

  const unauthorized =
    query.error instanceof ApiError && query.error.status === 401;

  useEffect(() => {
    if (unauthorized) {
      removeAccessToken();
    }
  }, [unauthorized]);

  return query;
}