namespace AntiClown.Entertainment.Api.Dto.Parties;

public class PartyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsOpened { get; set; }
    public int MaxPlayers { get; set; }
    public ulong RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime? FirstFullPartyAt { get; set; }
    public List<Guid> Participants { get; set; }
}