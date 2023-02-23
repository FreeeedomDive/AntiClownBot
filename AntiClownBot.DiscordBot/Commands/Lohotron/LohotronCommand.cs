using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Lohotron;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.Lohotron
{
    public class LohotronCommand : ICommand
    {
        public LohotronCommand(
            IDiscordClientWrapper discordClientWrapper,
            AntiClownDiscordBotVersion2.Models.Lohotron.Lohotron lohotron,
            IUserBalanceService userBalanceService,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.lohotron = lohotron;
            this.userBalanceService = userBalanceService;
            this.apiClient = apiClient;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            if (lohotron.UsersId.Contains(e.Author.Id))
            {
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    $"Чел, 2 раза нельзя! {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}"
                );
                return;
            }

            var prize = lohotron.Play(e.Author.Id);
            switch (prize.Name)
            {
                case "Credits":
                    var count = ((CreditsLohotronPrize)prize).Count;
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(e.Author.Id, count, "Лохотрон");
                    await discordClientWrapper.Messages.RespondAsync(e.Message, $"Ты получаешь {count} scam coins");
                    return;
                case "Nothing":
                    await discordClientWrapper.Messages.RespondAsync(e.Message,
                        $"Ты получаешь {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}!");
                    return;
                case "LootBox":
                    await discordClientWrapper.Messages.RespondAsync(e.Message, "Ты получаешь добычу-коробку!");
                    await apiClient.Items.AddLootBoxAsync(e.Author.Id);
                    return;
                default:
                    await discordClientWrapper.Messages.RespondAsync(e.Message, "Какой-то кал, всё сломалось");
                    return;
            }
        }

        public Task<string> Help()
        {
            return Task.FromResult("Ежедневно крутите лохотрон (1 раз)");
        }

        public string Name => "lohotron";
        public bool IsObsolete => true;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly AntiClownDiscordBotVersion2.Models.Lohotron.Lohotron lohotron;
        private readonly IUserBalanceService userBalanceService;
        private readonly IApiClient apiClient;
    }
}