using System.Text;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain.Inventory;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Inventories;

public class InventoryEmbedBuilder : IInventoryEmbedBuilder
{
    public InventoryEmbedBuilder(IUsersCache usersCache)
    {
        this.usersCache = usersCache;
    }

    public async Task<DiscordEmbed> BuildAsync(InventoryDetails inventoryDetails, IEnumerable<BaseItemDto> items)
    {
        var itemsById = items.ToDictionary(x => x.Id);
        var embedBuilder = new DiscordEmbedBuilder();
        var member = await usersCache.GetMemberByApiIdAsync(inventoryDetails.UserId);
        embedBuilder.WithTitle($"Предметы {member.ServerOrUserName()}");
        if (member is not null)
        {
            embedBuilder.WithColor(member.Color);
            embedBuilder.WithThumbnail(member.AvatarUrl);
        }

        embedBuilder.WithFooter($"Текущее действие: {ToolToString(inventoryDetails.Tool)}");

        if (inventoryDetails.Pages.Length == 0)
        {
            embedBuilder.AddField("Предметов нет...", "Совсем-совсем нет...");
            return embedBuilder.Build();
        }

        var currentPage = inventoryDetails.Pages[inventoryDetails.CurrentPage];
        embedBuilder.AddField(currentPage.ItemName.Localize(), currentPage.PageDescription);
        for (var i = 0; i < currentPage.ItemsIdsOnPage.Length; i++)
        {
            var item = itemsById[currentPage.ItemsIdsOnPage[i]];
            var fieldHeader = $"{i + 1}. {item.Rarity} {item.ItemName.Localize()}";
            var fieldDescriptionBuilder = new StringBuilder();
            fieldDescriptionBuilder.Append(item.IsActive ? "Активен" : "Не активен").Append('\n');
            fieldDescriptionBuilder.Append($"Цена: {item.Price}").Append('\n');
            fieldDescriptionBuilder.Append(string.Join("\n", item.Description().Select(kv => $"{kv.Key}: {kv.Value}")));
            embedBuilder.AddField(fieldHeader, fieldDescriptionBuilder.ToString());
        }

        return embedBuilder.Build();
    }

    private static string ToolToString(InventoryTool tool)
    {
        return tool switch
        {
            InventoryTool.Sell => "продажа",
            InventoryTool.ChangeActiveStatus => "изменение активности предмета",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private readonly IUsersCache usersCache;
}