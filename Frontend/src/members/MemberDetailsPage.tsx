import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getMember } from "./MembersApi";
import { memberKeys } from "./MemberKeys";
import { DeleteMemberButton } from "./DeleteMemberButton";

function readMemberId(value: string | undefined) {
    const memberId = Number(value);
    return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function MemberDetailsPage() {
    const { memberId: memberIdParameter } = useParams();
    const memberId = readMemberId(memberIdParameter);

    const memberQuery = useQuery ({
    queryKey: memberKeys.detail(memberId!),
    queryFn: () => {
        if (memberId === null){
        throw new Error("Invalid member id");
        }
     return getMember(memberId);
        },
        enabled: memberId !== null,
        retry: false,
    });

    if (memberId === null) {
        return (
            <main>
                <h1>Invalid member id</h1>
                <Link to="/members">Back to members</Link>
            </main>
        );
    }

    if (memberQuery.isPending)  {
        return <p>Loading member...</p>;
    }
    
    const notFound =
    memberQuery.error instanceof ApiError && memberQuery.error.status === 404;

    if(notFound) {
        return (
            <main>
                <h1>Member not found</h1>
                <p>The requested member does not exist.</p>
                <Link to="/members">Back to members</Link>
            </main>
        );
    }

    if(memberQuery.isError) {
    return (
    <main>
    <h1>Could not load the member</h1>
    <p>Is the Api running?</p>
    <Link to="/members">Back to members</Link>
    </main>
    );
   }

        const member = memberQuery.data;

        return (
        <main>
        <Link to="/members">Back to members</Link>
        <h1>{member.name}</h1>
        <p>Email: {member.email}</p>

        <p>
            <Link to={`/members/${member.id}/edit`}>Edit member</Link>
        </p>
        <DeleteMemberButton
        memberId={member.id}
        name={member.name}
        email={member.email}
      />
    </main>
  );
}
