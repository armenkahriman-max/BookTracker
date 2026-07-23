import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { login } from "./authApi";
import { setAccessToken } from "./tokenStorage";

export function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const loginMutation = useMutation({
    mutationFn: login,
    onSuccess: async (response) => {
      setAccessToken(response.accessToken);
      await queryClient.invalidateQueries({ queryKey: ["current-member"] });
      navigate("/account");
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    loginMutation.mutate({ email, password });
  }

  const invalidCredentials =
    loginMutation.error instanceof ApiError && loginMutation.error.status === 401;

  return (
    <main>
      <h1>Log in</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Email
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="email"
            required
          />
        </label>

        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            required
          />
        </label>

        <button type="submit" disabled={loginMutation.isPending}>
          {loginMutation.isPending ? "Logging in..." : "Log in"}
        </button>

        {invalidCredentials && <p>Email or password is incorrect.</p>}
        {loginMutation.isError && !invalidCredentials && (
          <p>Login failed. Is the API running?</p>
        )}
      </form>
    </main>
  );
}