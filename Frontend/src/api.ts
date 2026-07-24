import { getAccessToken } from "./auth/tokenStorage";

const apiUrl = import.meta.env.VITE_API_URL;

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

async function sendRequest(path: string, options: RequestInit) {
  const headers = new Headers(options.headers);
  const token = getAccessToken();

  headers.set("Accept", "application/json");

  if (options.body) {
    headers.set("Content-Type", "application/json");
  }

  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${apiUrl}${path}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    throw new ApiError(
      response.status,
      `Request failed with status ${response.status}`,
    );
  }

  return response;
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const response = await sendRequest(path, options);
  return response.json() as Promise<T>;
}

export async function apiRequestWithoutResponse(
  path: string,
  options: RequestInit = {},
): Promise<void> {
  await sendRequest(path, options);
}