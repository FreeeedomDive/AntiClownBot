using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Race;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;

namespace AntiClownDiscordBotVersion2.EventServices;

public class RaceService : IRaceService
{
    public RaceService(
        IDiscordClientWrapper discordClientWrapper,
        IRandomizer randomizer,
        IGuildSettingsService guildSettingsService,
        IApiClient apiClient
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.randomizer = randomizer;
        this.guildSettingsService = guildSettingsService;
        this.apiClient = apiClient;
    }

    public async Task CreateRaceAsync(ulong messageId)
    {
        Race = await new RaceModel(discordClientWrapper, randomizer, guildSettingsService, apiClient)
        {
            JoinableMessageId = messageId,
            OnRaceEnd = () => Race = null
        }.CreateAsync();
    }

    public RaceModel? Race { get; set; }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IRandomizer randomizer;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IApiClient apiClient;
}