namespace AntiClown.DiscordBot.Options;

public class DiscordOptions
{
    public string ApiToken { get; set; }
    public ulong GuildId { get; set; }
    public ulong AdminId { get; set; }
    public ulong BotChannelId { get; set; }
    public ulong TributeChannelId { get; set; }
    public ulong PartyChannelId { get; set; }
    public ulong HiddenTestChannelId { get; set; }
    public ulong DotaRoleId { get; set; }
    public ulong CsRoleId { get; set; }
    public ulong SiGameRoleId { get; set; }
    public int CreateRolePrice { get; set; }
    public int JoinRolePrice { get; set; }
}