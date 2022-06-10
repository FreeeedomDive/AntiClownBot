using AntiClownDiscordBotVersion2.Models.Gaming;

namespace AntiClownDiscordBotVersion2.Party;

public class PartiesInfo
{
    public Dictionary<ulong, GameParty> OpenParties { get; set; }
    public PartyStats PartyStats { get; set; }
    public List<ulong> JoinableRoles = new();
}