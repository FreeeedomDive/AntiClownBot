namespace AntiClown.Api.Core.Inventory.Domain;

public class ItemsFilter
{
    public Guid? OwnerId { get; set; }
    public bool? IsActive { get; set; }
    public string? Name { get; set; }
}