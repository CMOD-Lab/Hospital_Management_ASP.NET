namespace ClinicManagement.Domain.Exceptions;

/// <summary>Thrown when a requested entity is not found.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}

/// <summary>Thrown when a validation rule is violated.</summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>Thrown when a duplicate entry is detected.</summary>
public class DuplicateEntryException : Exception
{
    public DuplicateEntryException(string message) : base(message) { }
}
