using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Gaming;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Party;

public class PartyService : IPartyService
{
    public PartyService(
        IDiscordClientWrapper discordClientWrapper,
        IGuildSettingsService guildSettingsService
    )
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        fileName = $"{filesDirectory}/StatisticsFiles/parties.json";
        this.discordClientWrapper = discordClientWrapper;
        this.guildSettingsService = guildSettingsService;
        PartiesInfo = CreateOrRestore();
    }

    private async Task<DiscordEmbed> GetPartiesEmbed()
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        var embedBuilder = new DiscordEmbedBuilder();
        var partiesLinks = PartiesInfo
            .OpenParties
            .Values
            .ToDictionary(p => p, p => @$"https://discord.com/channels/{guildSettings.GuildId}/{guildSettings.PartyChannelId}/{p.MessageId}");

        embedBuilder.WithTitle("ТЕКУЩИЕ ПАТИ");
        if (PartiesInfo.OpenParties.Count == 0)
        {
            embedBuilder.Color = new Optional<DiscordColor>(DiscordColor.DarkRed);
            embedBuilder.AddField($"{await discordClientWrapper.Emotes.FindEmoteAsync("BibleThump")}",
                "Сейчас никто не играет");
        }
        else
        {
            embedBuilder.Color = new Optional<DiscordColor>(DiscordColor.DarkGreen);
            // ToList нужен для срабатывания ленивого foreach, так как без вызова неленивого метода коллекция не будет пройдена в цикле
            _ = partiesLinks.ForEach(
                (kv) => embedBuilder.AddField(
                    $"{kv.Key.Description} - {kv.Key.Players.Count} / {kv.Key.MaxPlayersCount} игроков",
                    @$"[Ссылка]({kv.Value})")).ToList();
        }

        return embedBuilder.Build();
    }

    public async Task<DiscordEmbed> CreatePartyEmbed()
    {
        return await GetPartiesEmbed();
    }

    public void DeleteObserverIfExists(DiscordMessage message)
    {
        if (partyObserver == null) return;
        if (partyObserver.Id != message.Id) return;
        partyObserver = null;
    }

    private PartiesInfo CreateOrRestore()
    {
        if (!File.Exists(fileName))
        {
            return new PartiesInfo
            {
                JoinableRoles = new List<ulong>(),
                OpenParties = new Dictionary<ulong, GameParty>(),
                PartyStats = new PartyStats()
            };
        }

        var parties = JsonConvert.DeserializeObject<PartiesInfo>(File.ReadAllText(fileName));
        if (parties == null)
        {
            return new PartiesInfo
            {
                JoinableRoles = new List<ulong>(),
                OpenParties = new Dictionary<ulong, GameParty>(),
                PartyStats = new PartyStats()
            };
        }

        var restoredParties = parties.OpenParties
            .ToDictionary(
                party => party.Key,
                party => GameParty.RestoreWithDependencies(
                    party.Value,
                    discordClientWrapper,
                    guildSettingsService,
                    RemoveOutdatedParty,
                    UpdateStatsAfterFullParty
                )
            );

        parties.OpenParties = restoredParties;

        return parties;
    }

    public async Task CreateNewParty(ulong authorId, string description, int maxPlayers, ulong? attachedRoleId = null)
    {
        var newParty = new GameParty(discordClientWrapper, guildSettingsService)
        {
            CreatorId = authorId,
            CreationDate = DateTime.Now,
            Description = description,
            MaxPlayersCount = maxPlayers,
            AttachedRoleId = attachedRoleId,
            OnPartyRemove = RemoveOutdatedParty,
            OnStatsUpdate = UpdateStatsAfterFullParty,
        };

        var message = await newParty.CreateMessage();
        await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, "YEP");
        await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, "MEGALUL");
        await discordClientWrapper.Messages.CreateThreadAsync(message, description);
        PartiesInfo.OpenParties.Add(message.Id, newParty);
    }

    public void RemoveOutdatedParty(ulong partyId)
    {
        PartiesInfo.OpenParties.Remove(partyId);
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(PartiesInfo, Formatting.Indented);
        File.WriteAllText(fileName, json);
    }

    private void UpdateStatsAfterFullParty(double seconds)
    {
        PartiesInfo.PartyStats.FastestPartyInSeconds = Math.Min(PartiesInfo.PartyStats.FastestPartyInSeconds, seconds);
        PartiesInfo.PartyStats.TotalFullParties++;
        PartiesInfo.PartyStats.TotalSeconds += seconds;
        Save();
    }

    public PartiesInfo PartiesInfo { get; }

    private static string fileName = "";
    private DiscordMessage? partyObserver;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IGuildSettingsService guildSettingsService;
}