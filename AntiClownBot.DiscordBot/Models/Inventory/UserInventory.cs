using System.Text;
using AntiClownApiClient;
using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models.Inventory
{
    public class UserInventory
    {
        public UserInventory(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.randomizer = randomizer;
        }

        public async Task<UserInventory> Create(
            ulong userId,
            DiscordMember member
        )
        {
            UserId = userId;
            Member = member;
            CurrentPage = 0;
            CurrentInventoryTool = InventoryTool.ChangeActiveStatus;
            UpdateItems(await RefreshItemsFromApi());

            return this;
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
                fieldDescriptionBuilder.Append(string.Join("\n", item.Description().Select(kv => $"{kv.Key}: {kv.Value}")));
                embedBuilder.AddField(fieldHeader, fieldDescriptionBuilder.ToString());
            }

            embedBuilder.WithFooter($"Текущее действие: {StringTool()}");

            return embedBuilder.Build();
        }

        public async Task SwitchLeftPage()
        {
            CurrentPage -= 1;
            if (CurrentPage < 0)
            {
                CurrentPage = Pages.Count - Math.Abs(CurrentPage);
            }
        }

        public async Task SwitchRightPage()
        {
            CurrentPage = (CurrentPage + 1) % Pages.Count;
        }

        public async Task EnableChangingStatus()
        {
            CurrentInventoryTool = InventoryTool.ChangeActiveStatus;
        }

        public async Task EnableSelling()
        {
            CurrentInventoryTool = InventoryTool.Sell;
        }

        public async Task<string> HandleItemInSlot(int slot)
        {
            if (slot > Pages[CurrentPage].PageItems.Length)
                return string.Empty;
            var item = Pages[CurrentPage].PageItems[slot - 1];
            switch (CurrentInventoryTool)
            {
                case InventoryTool.Sell:
                    return await SellItem(item);
                case InventoryTool.ChangeActiveStatus:
                    return await ChangeActiveStatusForItem(item);
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        private async Task<string> SellItem(BaseItem item)
        {
            var response = await apiClient.Items.SellItemAsync(UserId, item.Id);
            switch (response.Result)
            {
                case Enums.SellItemResult.Success:
                    UpdateItems(await RefreshItemsFromApi());
                    return string.Empty;
                case Enums.SellItemResult.NotEnoughMoney:
                    return "недостаточно денег для продажи негативного предмета";
                default:
                    throw new ArgumentOutOfRangeException(nameof(response.Result));
            }
        }

        private async Task<string> ChangeActiveStatusForItem(BaseItem item)
        {
            var response = await apiClient.Items.SetActiveStatusForItemAsync(UserId, item.Id, !item.IsActive);
            switch (response.Result)
            {
                case Enums.SetActiveStatusForItemResult.Success:
                    UpdateItems(await RefreshItemsFromApi());
                    return string.Empty;
                case Enums.SetActiveStatusForItemResult.TooManyActiveItems:
                    return "невозможно изменить статус предмета, активных предметов должно быть не более 3";
                case Enums.SetActiveStatusForItemResult.NegativeItemCantBeInactive:
                    return "негативный предмет нельзя сделать неактивным";
                default:
                    throw new ArgumentOutOfRangeException(nameof(response.Result));
            }
        }

        private async Task<IEnumerable<BaseItem>> RefreshItemsFromApi() => await apiClient.Items.AllItemsAsync(UserId);

        private string StringTool() => CurrentInventoryTool switch
        {
            InventoryTool.Sell => "продажа",
            InventoryTool.ChangeActiveStatus => "изменение активности предмета",
            _ => throw new ArgumentOutOfRangeException()
        };

        public ulong UserId { get; private set; }
        public DiscordMember Member { get; private set; }
        public List<UserInventoryPage> Pages { get; private set; }
        public int CurrentPage { get; private set; }
        public InventoryTool CurrentInventoryTool { get; set; }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IRandomizer randomizer;
    }
}