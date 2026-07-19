namespace ClinicManagement.Domain.Exceptions;

/// <summary>Base domain exception</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>Entity not found exception</summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found.") { }
}

/// <summary>Validation exception</summary>
public class ValidationException : DomainException
{
    public IEnumerable<string> Errors { get; }
    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

/// <summary>Duplicate email exception</summary>
public class DuplicateEmailException : DomainException
{
    public DuplicateEmailException(string email)
        : base($"Email '{email}' already exists.") { }
}
