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