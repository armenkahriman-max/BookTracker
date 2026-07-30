import { keepPreviousData, useQuery } from "@tanstack/react-query";
import type { FormEvent } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getMembers } from "./MembersApi";
import { memberKeys } from "./MemberKeys";

const pageSize = 10;

function readPage(value: string | null) {
  const page = Number(value);
  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function MemberListPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = readPage(searchParams.get("page"));
  const search = searchParams.get("search")?.trim() ?? "";

  const membersQuery = useQuery({
    queryKey: memberKeys.list({ page, pageSize, search }),
    queryFn: () => getMembers({ page, pageSize, search }),
    placeholderData: keepPreviousData,
  });

  function setPage(nextPage: number) {
    const next = new URLSearchParams(searchParams);

    if (nextPage === 1) {
      next.delete("page");
    } else {
      next.set("page", nextPage.toString());
    }

    setSearchParams(next);
  }

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const next = new URLSearchParams();
    const formData = new FormData(event.currentTarget);
    const value = formData.get("search")?.toString().trim() ?? "";

    if (value) {
      next.set("search", value);
    }

    setSearchParams(next);
  }

  if (membersQuery.isPending) {
    return <p>Loading members...</p>;
  }

  if (membersQuery.isError) {
    return <p>Could not load the members. Is the API running?</p>;
  }

  const result = membersQuery.data;

  return (
    <main>
      <h1>Members</h1>

      <form key={search} onSubmit={handleSearch}>
        <label>
          Search by name or email
          <input
            type="search"
            name="search"
            defaultValue={search}
          />
        </label>
        <button type="submit">Search</button>
      </form>

      {result.items.length === 0 ? (
        <p>No members found.</p>
      ) : (
        <ul>
          {result.items.map((member) => (
            <li key={member.id}>
              <Link to={`/members/${member.id}`}>
                <strong>{member.name}</strong> — {member.email}
              </Link>
            </li>
          ))}
        </ul>
      )}

      <p>
        Page {result.page} of {result.totalPages}. {result.totalItems} members
        found.
      </p>

      <button
        type="button"
        onClick={() => setPage(result.page - 1)}
        disabled={result.page <= 1 || membersQuery.isFetching}
      >
        Previous
      </button>{" "}

      <button
        type="button"
        onClick={() => setPage(result.page + 1)}
        disabled={
          result.page >= result.totalPages || membersQuery.isFetching
        }
      >
        Next
      </button>

      {membersQuery.isFetching && <p>Updating members...</p>}
    </main>
  );
}