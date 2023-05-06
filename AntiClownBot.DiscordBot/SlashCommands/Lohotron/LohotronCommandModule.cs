using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.Models.Lohotron;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Lohotron
{
    public class LohotronCommandModule : SlashCommandModuleWithMiddlewares
    {
        public LohotronCommandModule(
            ICommandExecutor commandExecutor,
            IDiscordClientWrapper discordClientWrapper,
            Models.Lohotron.Lohotron lohotron,
            IUserBalanceService userBalanceService,
            IApiClient apiClient
        ) : base(commandExecutor)
        {
            this.discordClientWrapper = discordClientWrapper;
            this.lohotron = lohotron;
            this.userBalanceService = userBalanceService;
            this.apiClient = apiClient;
        }

        [SlashCommand(Interactions.Commands.Lohotron, "Прокрутить колесо лохотрона (1 раз в день)")]
        public async Task PlayLohotron(InteractionContext context)
        {
            await ExecuteAsync(context, async () =>
            {
                var userId = context.Member.Id;
                if (lohotron.UsersId.Contains(userId))
                {
                    await RespondToInteractionAsync(context, $"Чел, 2 раза нельзя! {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}");
                    return;
                }

                var prize = lohotron.Play(userId);
                lohotron.UsersId.Add(userId);
                switch (prize.Name)
                {
                    case "Credits":
                        var count = ((CreditsLohotronPrize)prize).Count;
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, count, "Лохотрон");
                        await RespondToInteractionAsync(context, $"Ты получаешь {count} scam coins");
                        return;
                    case "Nothing":
                        await RespondToInteractionAsync(context, $"Ты получаешь {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}!");
                        return;
                    case "LootBox":
                        await RespondToInteractionAsync(context, "Ты получаешь добычу-коробку!");
                        await apiClient.Items.AddLootBoxAsync(userId);
                        return;
                    default:
                        throw new ArgumentException($"Unknown prizeName {prize.Name}");
                }
            });
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly Models.Lohotron.Lohotron lohotron;
        private readonly IUserBalanceService userBalanceService;
        private readonly IApiClient apiClient;
    }
}
