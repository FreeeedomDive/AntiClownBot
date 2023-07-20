namespace AntiClown.Entertainment.Api.Dto.Parties;

public class CreatePartyDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxPlayers { get; set; }
    public Guid CreatorId { get; set; }
    public ulong RoleId { get; set; }
}