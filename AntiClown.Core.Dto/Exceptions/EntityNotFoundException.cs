namespace AntiClown.Core.Dto.Exceptions;

public class EntityNotFoundException : AntiClownNotFoundException
{
    public EntityNotFoundException(string entityName) : base($"Entity with name {entityName} does not exist")
    {
        EntityName = entityName;
    }

    public string EntityName { get; }
}