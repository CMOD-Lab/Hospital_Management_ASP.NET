namespace ClinicManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found.")
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}
