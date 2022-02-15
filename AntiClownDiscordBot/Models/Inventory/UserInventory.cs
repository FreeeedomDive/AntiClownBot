using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Helpers;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses;
using ApiWrapper.Wrappers;
using DSharpPlus.Entities;

namespace AntiClownBot.Models.Inventory
{
    public class UserInventory
    {
        public ulong UserId { get; }
        public DiscordMessage Message { get; init; }
        public DiscordMember Member { get; init; }
        public List<UserInventoryPage> Pages { get; private set; }
        public int CurrentPage { get; private set; }
        public Instrument CurrentInstrument { get; set; }

        public UserInventory(ulong userId)
        {
            UserId = userId;
            CurrentPage = 0;
            CurrentInstrument = Instrument.ChangeActiveStatus;
            UpdateItems(RefreshItemsFromApi());
        }

        private void UpdateItems(IEnumerable<BaseItem> items)
        {
            const int itemsPerPage = 5;
            Pages = new List<UserInventoryPage>();
            var orderedGroupsByName = items.GroupBy(item => item.Name);
            foreach (var nameGroup in orderedGroupsByName)
            {
                var groupItems = nameGroup.ToList();
                var chunkItems = groupItems.Chunk(itemsPerPage);
                var offset = 0;
                _ = chunkItems.ForEach(pageItems =>
                {
                    var currentOffset = offset;
                    Pages.Add(new UserInventoryPage(
                        nameGroup.Key,
                        $"Предметы {currentOffset + 1}-{currentOffset + pageItems.Length} из {groupItems.Count}",
                        pageItems));
                    offset += pageItems.Length;
                }).ToArray();
            }
        }

        public DiscordEmbed UpdateEmbedForCurrentPage()
        {
            var page = CurrentPage < Pages.Count ? Pages[CurrentPage] : Pages.Last();
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithColor(Member.Color);
            embedBuilder.WithThumbnail(Member.AvatarUrl);
            embedBuilder.WithTitle($"Предметы {Member.ServerOrUserName()}");
            embedBuilder.AddField(page.ItemType, page.PageDescription);
            for (var i = 0; i < page.PageItems.Length; i++)
            {
                var item = page.PageItems[i];
                var fieldHeader = $"{i + 1}. {item.Rarity} {item.Name}";
                var fieldDescriptionBuilder = new StringBuilder();
                fieldDescriptionBuilder.Append(item.IsActive ? "Активен" : "Не активен").Append('\n');
                fieldDescriptionBuilder.Append($"Цена: {item.Price}").Append('\n');
                fieldDescriptionBuilder.Append(string.Join("\n",
                    item.Description().Select(kv => $"{kv.Key}: {kv.Value}")));
                embedBuilder.AddField(fieldHeader, fieldDescriptionBuilder.ToString());
            }

            embedBuilder.WithFooter($"Текущее действие: {StringInstrument()}");

            return embedBuilder.Build();
        }

        public async void SwitchLeftPage()
        {
            CurrentPage -= 1;
            if (CurrentPage < 0)
            {
                CurrentPage = Pages.Count - Math.Abs(CurrentPage);
            }
            await Message.ModifyAsync(UpdateEmbedForCurrentPage());
        }

        public async void SwitchRightPage()
        {
            CurrentPage = (CurrentPage + 1) % Pages.Count;
            await Message.ModifyAsync(UpdateEmbedForCurrentPage());
        }

        public async void EnableChangingStatus()
        {
            CurrentInstrument = Instrument.ChangeActiveStatus;
            await Message.ModifyAsync(UpdateEmbedForCurrentPage());
        }

        public async void EnableSelling()
        {
            CurrentInstrument = Instrument.Sell;
            await Message.ModifyAsync(UpdateEmbedForCurrentPage());
        }
        
        public void HandleItemInSlot(int slot)
        {
            if (slot > Pages[CurrentPage].PageItems.Length)
                return;
            var item = Pages[CurrentPage].PageItems[slot - 1];
            switch (CurrentInstrument)
            {
                case Instrument.Sell:
                    SellItem(item);
                    break;
                case Instrument.ChangeActiveStatus:
                    ChangeActiveStatusForItem(item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        private async void SellItem(BaseItem item)
        {
            var response = ItemsApi.SellItem(UserId, item.Id);
            switch (response.Result)
            {
                case Enums.SellItemResult.Success:
                    UpdateItems(RefreshItemsFromApi());
                    await Message.ModifyAsync(UpdateEmbedForCurrentPage());
                    break;
                case Enums.SellItemResult.NotEnoughMoney:
                    RespondWithMention("недостаточно денег для продажи негативного предмета");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(response.Result));
            }
        }

        private async void ChangeActiveStatusForItem(BaseItem item)
        {
            var response = ItemsApi.SetActiveStatusForItem(UserId, item.Id, !item.IsActive);
            switch (response.Result)
            {
                case Enums.SetActiveStatusForItemResult.Success:
                    UpdateItems(RefreshItemsFromApi());
                    await Message.ModifyAsync(UpdateEmbedForCurrentPage());
                    break;
                case Enums.SetActiveStatusForItemResult.TooManyActiveItems:
                    RespondWithMention("невозможно изменить статус предмета, активных предметов должно быть не более 3");
                    break;
                case Enums.SetActiveStatusForItemResult.NegativeItemCantBeInactive:
                    RespondWithMention("негативный предмет нельзя сделать неактивным");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(response.Result));
            }
        }

        private async void RespondWithMention(string message)
        {
            var responseBuilder = new DiscordMessageBuilder();
            responseBuilder.WithAllowedMention(UserMention.All);
            responseBuilder.Content = $"{Member.Mention} {message}";
            await Message.RespondAsync(responseBuilder);
        }

        private IEnumerable<BaseItem> RefreshItemsFromApi() => ItemsApi.AllItems(UserId);
        
        public static DiscordEmbed CreateLoadingInventoryEmbed()
        {
            var loadingEmotes = new List<string>()
            {
                $"{Utility.Emoji(":pigRoll:")}",
                $"{Utility.Emoji(":Applecatrun:")}",
                $"{Utility.Emoji(":SCAMMED:")}",
                $"{Utility.Emoji(":COGGERS:")}",
                $"{Utility.Emoji(":RainbowPls:")}",
                $"{Utility.Emoji(":PolarStrut:")}",
                $"{Utility.Emoji(":popCat:")}",
            };

            var loadingEmbedBuilder = new DiscordEmbedBuilder();
            loadingEmbedBuilder.WithTitle($"Загрузка инвентаря... {loadingEmotes.SelectRandomItem().Multiply(5)}");

            return loadingEmbedBuilder.Build();
        }

        private string StringInstrument() => CurrentInstrument switch
        {
            Instrument.Sell => "продажа",
            Instrument.ChangeActiveStatus => "изменение активности предмета",
            _ => throw new ArgumentOutOfRangeException()
        };

        public enum Instrument
        {
            Sell,
            ChangeActiveStatus
        }
    }
}