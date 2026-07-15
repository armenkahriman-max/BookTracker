using BookTracker.Api.Domain;

namespace BookTracker.Api.Application.Members;

public class MemberEmailAlreadyExistsException()
    : DomainException("A member with this email already exists.");