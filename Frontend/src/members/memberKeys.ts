export const memberKeys = {
    all: ['members'] as const,
    lists: () =>[...memberKeys.all, 'list'] as const,
    list: (params: { page: number; pageSize: number; search: string }) =>
    [...memberKeys.lists(), params] as const,
    details: () => [...memberKeys.all, 'details'] as const,
    detail: (id: number) => [...memberKeys.details(), id] as const,
};

export const authKeys = {
    me:["auth","me"] as const,
};