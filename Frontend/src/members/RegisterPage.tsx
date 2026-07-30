import { useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { registerMember } from "./MembersApi";

export function RegisterPage() {
  const [formError, setFormError] = useState<string | null>(null);
  const navigate = useNavigate();

  const registerMutation = useMutation({
    mutationFn: registerMember,
    onSuccess: (member) => {
      navigate("/login", {
        state: {
          registered: true,
          email: member.email,
        },
      });
    },
  });

   function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";
    const password = formData.get("password")?.toString() ?? "";
    const confirmPassword =
      formData.get("confirmPassword")?.toString() ?? "";

    if (!name || !email || !password) {
      setFormError("Name, email and password are required.");
      return;
    }

    if (password.length < 8) {
      setFormError("Password must contain at least 8 characters.");
      return;
    }

    if (password !== confirmPassword) {
      setFormError("Passwords do not match.");
      return;
    }

    registerMutation.mutate({ name, email, password });
  }
  const badRequest =
    registerMutation.error instanceof ApiError &&
    registerMutation.error.status === 400;

  const duplicateEmail =
    registerMutation.error instanceof ApiError &&
    registerMutation.error.status === 409;

  return (
    <main>
      <h1>Create account</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Name
          <input
            name="name"
            autoComplete="name"
            maxLength={100}
            required
          />
        </label>

        <label>
          Email
          <input
            name="email"
            type="email"
            autoComplete="email"
            maxLength={200}
            required
          />
        </label>

        <label>
          Password
          <input
            name="password"
            type="password"
            autoComplete="new-password"
            minLength={8}
            required
          />
        </label>

        <label>
          Confirm password
          <input
            name="confirmPassword"
            type="password"
            autoComplete="new-password"
            minLength={8}
            required
          />
        </label>

        <button type="submit" disabled={registerMutation.isPending}>
          {registerMutation.isPending ? "Creating account..." : "Register"}
        </button>
      </form>

      {formError && <p>{formError}</p>}
      {badRequest && <p>The API rejected the registration data.</p>}
      {duplicateEmail && <p>An account with this email already exists.</p>}
      {registerMutation.isError && !badRequest && !duplicateEmail && (
        <p>Could not create the account. Is the API running?</p>
      )}

      <p>
        Already have an account? <Link to="/login">Log in</Link>
      </p>
    </main>
  );
}