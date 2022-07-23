using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Lottery;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;

namespace AntiClownDiscordBotVersion2.EventServices;

public class LotteryService : ILotteryService
{
    public LotteryService(
        IDiscordClientWrapper discordClientWrapper,
        IRandomizer randomizer,
        IEventSettingsService eventSettingsService,
        IGuildSettingsService guildSettingsService,
        IApiClient apiClient,
        IUserBalanceService userBalanceService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.randomizer = randomizer;
        this.eventSettingsService = eventSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.apiClient = apiClient;
        this.userBalanceService = userBalanceService;
    }

    public void CreateLottery(ulong messageId)
    {
        Lottery = new Lottery(
            discordClientWrapper,
            randomizer,
            eventSettingsService,
            guildSettingsService,
            apiClient,
            userBalanceService
        )
        {
            OnLotteryEnd = () => Lottery = null
        }.Create(messageId);
    }

    public Lottery? Lottery { get; set; }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IRandomizer randomizer;
    private readonly IEventSettingsService eventSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
}