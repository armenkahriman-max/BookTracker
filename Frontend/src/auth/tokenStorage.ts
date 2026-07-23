const tokenKey = "book-tracker-access-token";

export function getAccessToken() {
  return localStorage.getItem(tokenKey);
}

export function setAccessToken(token: string) {
  localStorage.setItem(tokenKey, token);
}

export function removeAccessToken() {
  localStorage.removeItem(tokenKey);
}