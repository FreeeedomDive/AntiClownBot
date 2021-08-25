using System;
using System.Collections.Generic;
using System.Linq;
using ApiWrapper.Constants;
using ApiWrapper.Models.Items;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class RatingCommand : BaseCommand
    {
        public RatingCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var member = Configuration.GetServerMember(e.Author.Id);
            var response = ApiWrapper.Wrappers.UsersApi.Rating(e.Author.Id);
            var netWorth = response.ScamCoins + response.Inventory.Where(item => item.ItemType == ItemType.Positive)
                .Select(item => item.Price).Sum();

            var embedBuilder = new DiscordEmbedBuilder
            {
                Color = member.Color
            };

            var name = member.ServerOrUsername();
            embedBuilder.WithThumbnail(e.Author.AvatarUrl);
            embedBuilder.WithTitle(
                $"ЧЕЛА РЕАЛЬНО ЗОВУТ {name.ToUpper()}" +
                $" {Utility.Emoji(":aRolf:")}" +
                $" {Utility.Emoji(":aRolf:")}" +
                $" {Utility.Emoji(":aRolf:")}");

            embedBuilder.AddField("SCAM COINS", $"{response.ScamCoins}");
            embedBuilder.AddField("Общая ценность", $"{netWorth}");

            foreach (var itemName in StringConstants.AllItemsNames)
            {
                var itemsOfType = response.Inventory.Where(item => item.Name.Equals(itemName)).ToList();
                var descriptions = itemsOfType.Count == 0
                    ? "Нет предметов"
                    : $"{string.Join(" ", itemsOfType.Select(item => $"{item.Rarity}"))}\n" +
                      $"{string.Join("\n", CalculateItemStats(itemsOfType, itemName).Select(kv => $"{kv.Key}: {kv.Value}"))}";
                embedBuilder.AddField(
                    $"{itemName} - {itemsOfType.Count}",
                    descriptions);
            }

            await e.Message.RespondAsync(embedBuilder.Build());
        }

        private static Dictionary<string, string> CalculateItemStats(List<BaseItem> items, string itemType)
        {
            return itemType switch
            {
                StringConstants.CatWifeName => new Dictionary<string, string>()
                {
                    {
                        "Шанс на автоматическое подношение",
                        $"{items.Where(i => i.Name == itemType).Select(i => (CatWife) i).Select(i => i.AutoTributeChance).Sum()}%"
                    }
                },
                StringConstants.DogWifeName => new Dictionary<string, string>()
                {
                    {
                        "Шанс получить лутбокс во время подношения",
                        $"{(double) items.Where(i => i.Name == itemType).Select(i => (DogWife) i).Select(i => i.LootBoxFindChance).Sum() / 10}%"
                    }
                },
                StringConstants.RiceBowlName => new Dictionary<string, string>()
                {
                    {
                        "Границы получения подношения",
                        $"от {NumericConstants.MinTributeValue - items.Where(i => i.Name == itemType).Select(i => (RiceBowl) i).Select(i => i.NegativeRangeExtend).Sum()}"
                        + $" до {NumericConstants.MaxTributeValue + items.Where(i => i.Name == itemType).Select(i => (RiceBowl) i).Select(i => i.PositiveRangeExtend).Sum()}"
                    }
                },
                StringConstants.InternetName => new Dictionary<string, string>()
                {
                    {
                        "Шанс уменьшения кулдауна во время одной попытки",
                        $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (Internet) i).Select(i => $"{i.Speed}%"))}"
                    },
                    {
                        "Общее количество попыток уменьшить кулдаун",
                        $"{items.Where(i => i.Name == itemType).Select(i => (Internet) i).Select(i => i.Gigabytes).Sum()}"
                    },
                    {
                        "Уменьшение кулдауна в процентах",
                        $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (Internet) i).Select(i => $"{i.Ping}%"))}"
                    }
                },
                StringConstants.JadeRodName => new Dictionary<string, string>()
                {
                    {
                        "Шанс увеличения кулдауна во время одной попытки",
                        $"{NumericConstants.CooldownIncreaseChanceByOneJade}%"
                    },
                    {
                        "Общее количество попыток увеличить кулдаун",
                        $"{items.Where(i => i.Name == itemType).Select(i => (JadeRod) i).Select(i => i.Length).Sum()}"
                    },
                    {
                        "Увеличение кулдауна в процентах",
                        $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (JadeRod) i).Select(i => $"{i.Thickness}%"))}"
                    }
                },
                StringConstants.CommunismBannerName => new Dictionary<string, string>()
                {
                    {
                        "Шанс разделить награду за подношение с другим владельцем плаката",
                        $"{items.Where(i => i.Name == itemType).Select(i => (CommunismBanner) i).Select(i => i.DivideChance).Sum()}%"
                    },
                    {
                        "Приоритет стащить чужое подношение (если у него сработал плакат)",
                        $"{items.Where(i => i.Name == itemType).Select(i => (CommunismBanner) i).Select(i => i.StealChance).Sum()}"
                    }
                },

                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
        }

        public override string Help()
        {
            return "Получение своего паспорта с информацией о балансе и предметах";
        }
    }
}