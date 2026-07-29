export type RegisterMemberRequest = {
  name: string;
  email: string;
  password: string;
};

export type RegisterMemberResponse = {
  id: number;
  name: string;
  email: string;
};

export type Member = {
  id: number;
  name: string;
  email: string;
};

export type CurrentMember = Member & {
role: 'Member' | 'Administrator';
};



export type UpdateMemberRequest = {
  name: string;
  email: string;
};
export type GetMembersRequest = {
  page: number;
  pageSize: number;
  search?: string;
}