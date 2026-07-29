import { useQuery, useMutation, useQueryClient, keepPreviousData } from "@tanstack/react-query";
import {
getMe,
getMembers,
getMember,
updateMember,
deleteMember,
} from "./membersApi";
import type { GetMembersRequest, UpdateMemberRequest } from "./types";
import { memberKeys, authKeys } from "./memberKeys";

export function useCurrentMember() {
    return useQuery({
        queryKey: authKeys.me,
        queryFn: getMe,
        staleTime: 5 * 60 * 1000,
    });
}

export function useMembers(request: GetMembersRequest) {
    return useQuery({
        queryKey: memberKeys.list({
            page: request.page,
            pageSize: request.pageSize,
            search: request.search ?? "",
        }),
        queryFn: () => getMembers(request),
        placeholderData:keepPreviousData,
    });
}

export function useMember(memberId: number | undefined) {
    return useQuery ({
        queryKey: memberKeys.detail(memberId!),
        queryFn: () => getMember(memberId!),
        enabled: typeof memberId === "number" && memberId > 0,
    });
}
  export function useUpdateMember() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      memberId,
      request,
    }: {
      memberId: number;
      request: UpdateMemberRequest;
    }) => updateMember(memberId, request),

    onSuccess: (_data, { memberId }) => {
     
      queryClient.invalidateQueries({ queryKey: memberKeys.lists() });

  
      queryClient.invalidateQueries({ queryKey: memberKeys.detail(memberId) });

      
      queryClient.invalidateQueries({ queryKey: authKeys.me });
    },
  });
}

   export function useDeleteMember() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (memberId: number) => deleteMember(memberId),

    onSuccess: ( _data, memberId) => {
      
      queryClient.removeQueries({ queryKey: memberKeys.detail(memberId) });

    
      queryClient.invalidateQueries({ queryKey: memberKeys.lists() });

      
    },
  });
}
