import { useSyncExternalStore } from "react";

const tokenKey = "book-tracker-access-token";
const listeners = new Set<() => void>();

function notifyListeners() {
  listeners.forEach((listener) => listener());
}

function subscribe(listener: () => void) {
  listeners.add(listener);

  function handleStorage(event: StorageEvent) {
    if (event.key === tokenKey) listener();
  }

  window.addEventListener("storage", handleStorage);
  return () => {
    listeners.delete(listener);
    window.removeEventListener("storage", handleStorage);
  };
}

export function getAccessToken() {
  return localStorage.getItem(tokenKey);
}

export function useAccessToken() {
  return useSyncExternalStore(subscribe, getAccessToken, () => null);
}

export function setAccessToken(token: string) {
  localStorage.setItem(tokenKey, token);
  notifyListeners();
}

export function removeAccessToken() {
  localStorage.removeItem(tokenKey);
  notifyListeners();
}