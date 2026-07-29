import { apiRequest, apiRequestWithoutResponse } from "../api";
import type { PagedResult } from "../types";
import type {
  Member,
  CurrentMember,
  UpdateMemberRequest,
  GetMembersRequest,
  RegisterMemberRequest,
  RegisterMemberResponse,
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
export function getMe() {
  return apiRequest<CurrentMember>("/auth/me");
}
export function getMembers(request: GetMembersRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<Member>>(
    `/members?${parameters.toString()}`
  );
}

export function getMember(memberId: number) {
  return apiRequest<Member>(`/members/${memberId}`);
}

export function updateMember(memberId: number, request: UpdateMemberRequest) {
  return apiRequestWithoutResponse(`/members/${memberId}` , {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function deleteMember(memberId: number) {
  return apiRequestWithoutResponse(`/members/${memberId}`, {
    method: "DELETE"
  });
}