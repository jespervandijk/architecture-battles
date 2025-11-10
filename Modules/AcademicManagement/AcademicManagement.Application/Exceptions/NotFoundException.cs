namespace AcademicManagement.Application.Exceptions;

public class EntityNotFoundException : InvalidOperationException
{
    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} with ID '{entityId}' not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public string EntityName { get; }
    public object EntityId { get; }
}
