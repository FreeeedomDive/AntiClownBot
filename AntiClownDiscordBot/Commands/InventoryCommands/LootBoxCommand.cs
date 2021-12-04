using System.Linq;
using AntiClownBot.Helpers;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.InventoryCommands
{
    public class LootBoxCommand : BaseCommand
    {
        public LootBoxCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var result = ItemsApi.OpenLootBox(e.Author.Id);
            if (!result.IsSuccessful)
            {
                await e.Message.RespondAsync($"Нет доступных лутбоксов {Utility.Emoji(":modCheck:")}");
                return;
            }

            var member = Configuration.GetServerMember(e.Author.Id);
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle(
                $"{member.ServerOrUserName()} открывает лутбокс...");
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

            await e.Message.RespondAsync(embedBuilder.Build());
        }

        public override string Help() => "Открытие лутбокса";
    }
}