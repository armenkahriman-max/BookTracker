import { apiRequest } from "../api";
import type {
  RegisterMemberRequest,
  RegisterMemberResponse,
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
}