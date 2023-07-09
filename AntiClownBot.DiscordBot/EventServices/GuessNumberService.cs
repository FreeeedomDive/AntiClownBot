using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Models.GuessNumber;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;

namespace AntiClownDiscordBotVersion2.EventServices;

public class GuessNumberService : IGuessNumberService
{
    public GuessNumberService(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesProvider emotesProvider,
        IGuildSettingsService guildSettingsService,
        IRandomizer randomizer,
        IApiClient apiClient
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesProvider = emotesProvider;
        this.guildSettingsService = guildSettingsService;
        this.randomizer = randomizer;
        this.apiClient = apiClient;
    }

    public void CreateGuessNumberGame(ulong messageId)
    {
        CurrentGame = new GuessNumberGame(discordClientWrapper, emotesProvider, guildSettingsService, randomizer, apiClient)
        {
            GuessNumberGameMessageMessageId = messageId,
            OnGameEnd = () => CurrentGame = null
        }.Create();
    }

    public GuessNumberGame? CurrentGame { get; set; }
    
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesProvider emotesProvider;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IRandomizer randomizer;
    private readonly IApiClient apiClient;
}