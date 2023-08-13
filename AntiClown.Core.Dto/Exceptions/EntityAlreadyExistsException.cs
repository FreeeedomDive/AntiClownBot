namespace AntiClown.Core.Dto.Exceptions;

public class EntityAlreadyExistsException : AntiClownConflictException
{
    public EntityAlreadyExistsException(string entityName) : base($"Entity with name {entityName} already exists")
    {
        EntityName = entityName;
    }

    public string EntityName { get; }
}