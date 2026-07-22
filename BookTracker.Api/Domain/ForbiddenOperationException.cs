namespace BookTracker.Api.Domain;

public class ForbiddenOperationException(
    string message) : Exception(message);