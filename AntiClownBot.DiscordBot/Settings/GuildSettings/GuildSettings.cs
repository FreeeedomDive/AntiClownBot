namespace AntiClownDiscordBotVersion2.Settings.GuildSettings;

public class GuildSettings
{
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
    
    public string MinecraftServerLocalAddress { get; set; }
    public int MinecraftServerPort { get; set; }
    public string MinecraftServerFolder { get; set; }
    
    public string CommandsPrefix { get; set; }
}