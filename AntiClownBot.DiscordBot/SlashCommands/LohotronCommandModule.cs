﻿using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Lohotron;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands
{
    public class LohotronCommandModule : ApplicationCommandModule
    {
        public LohotronCommandModule(
            IDiscordClientWrapper discordClientWrapper,
            Lohotron lohotron,
            IUserBalanceService userBalanceService,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.lohotron = lohotron;
            this.userBalanceService = userBalanceService;
            this.apiClient = apiClient;
        }

        [SlashCommand("lohotron", "Прокрутить колесо лохотрона (1 раз в день)")]
        public async Task CreateParty(InteractionContext context)
        {
            if (lohotron.UsersId.Contains(context.Member.Id))
            {
                await discordClientWrapper.Messages.RespondAsync(
                    context,
                    $"Чел, 2 раза нельзя! {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}"
                );
                return;
            }

            var prize = lohotron.Play();
            lohotron.UsersId.Add(context.Member.Id);
            switch (prize.Name)
            {
                case "Credits":
                    var count = ((CreditsLohotronPrize)prize).Count;
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(context.Member.Id, count, "Лохотрон");
                    await discordClientWrapper.Messages.RespondAsync(context, $"Ты получаешь {count} scam coins");
                    return;
                case "Nothing":
                    await discordClientWrapper.Messages.RespondAsync(context,
                        $"Ты получаешь {await discordClientWrapper.Emotes.FindEmoteAsync("peepoFinger")}!");
                    return;
                case "LootBox":
                    await discordClientWrapper.Messages.RespondAsync(context, "Ты получаешь добычу-коробку!");
                    await apiClient.Items.AddLootBoxAsync(context.Member.Id);
                    return;
                default:
                    await discordClientWrapper.Messages.RespondAsync(context, "Какой-то кал, всё сломалось");
                    return;
            }
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly Lohotron lohotron;
        private readonly IUserBalanceService userBalanceService;
        private readonly IApiClient apiClient;
    }
}