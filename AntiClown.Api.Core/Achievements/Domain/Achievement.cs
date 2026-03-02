namespace AntiClown.Api.Core.Achievements.Domain;

public record Achievement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[] Logo { get; set; } = [];
}
