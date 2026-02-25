namespace TrackIt.Application.Common.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with key '{key}' was not found.");

public class ForbiddenException(string message) : Exception(message);

public class ValidationException(IEnumerable<string> errors)
    : Exception("One or more validation errors occurred.")
{
    public IEnumerable<string> Errors { get; } = errors;
}

public class ConflictException(string message) : Exception(message);

public class UnauthorizedException(string message) : Exception(message);
