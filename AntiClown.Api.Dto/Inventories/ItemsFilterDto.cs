namespace AntiClown.Api.Dto.Inventories;

public class ItemsFilterDto
{
    public Guid? OwnerId { get; set; }
    public bool? IsActive { get; set; }
    public string? Name { get; set; }
}