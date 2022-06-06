using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Party;

public interface IPartyService
{
    Task CreateNewParty(ulong authorId, string description, int maxPlayers, ulong? attachedRoleId = null);
    void RemoveOutdatedParty(ulong partyId);

    Task AddPartyObserverMessage(DiscordMessage messageToRespond);
    void DeleteObserverIfExists(DiscordMessage message);

    void Save();

    PartiesInfo PartiesInfo { get; }
}