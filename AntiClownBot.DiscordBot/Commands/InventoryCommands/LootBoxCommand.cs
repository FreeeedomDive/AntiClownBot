using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.InventoryCommands
{
    public class LootBoxCommand : ICommand
    {
        public LootBoxCommand(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var result = await apiClient.Items.OpenLootBoxAsync(e.Author.Id);
            if (!result.IsSuccessful)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    $"Нет доступных лутбоксов {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}"
                );
                return;
            }

            var member = await discordClientWrapper.Members.GetAsync(e.Author.Id);
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle($"{member.ServerOrUserName()} открывает лутбокс...");
            embedBuilder.WithColor(member.Color);

            var reward = result.Reward;
            embedBuilder.AddField("Денежное вознаграждение", $"Ты получил {reward.ScamCoinsReward} scam coins!");

            var itemIndex = 1;
            foreach (var item in reward.Items)
            {
                embedBuilder.AddField($"{itemIndex++} предмет", $"Ты получил {item.Rarity} {item.Name}\n" +
                                                                string.Join("\n",
                                                                    item
                                                                        .Description()
                                                                        .Select(kv => $"{kv.Key}: {kv.Value}")));
            }

            await discordClientWrapper.Messages.RespondAsync(e.Message, embedBuilder.Build());
        }

        public Task<string> Help()
        {
            return Task.FromResult("Открытие лутбокса");
        }

        public string Name => "lootbox";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
    }
}