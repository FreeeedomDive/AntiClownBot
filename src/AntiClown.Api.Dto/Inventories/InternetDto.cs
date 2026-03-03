namespace AntiClown.Api.Dto.Inventories;

public class InternetDto : BaseItemDto
{
    public int Gigabytes { get; set; }
    public int Speed { get; set; }
    public int Ping { get; set; }
}