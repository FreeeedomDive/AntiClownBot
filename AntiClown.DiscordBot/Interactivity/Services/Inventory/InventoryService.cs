using AntiClown.Api.Client;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Items;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Inventories;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.Inventory;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.Interactivity.Services.Inventory;

public class InventoryService : IInventoryService
{
    public InventoryService(
        IInteractivityRepository interactivityRepository,
        IInventoryEmbedBuilder inventoryEmbedBuilder,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache,
        IEmotesCache emotesCache
    )
    {
        this.interactivityRepository = interactivityRepository;
        this.inventoryEmbedBuilder = inventoryEmbedBuilder;
        this.antiClownApiClient = antiClownApiClient;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
    }

    public async Task CreateAsync(InteractionContext context, Func<InteractionContext, DiscordWebhookBuilder, Task<DiscordMessage>> createMessage)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
        var inventory = await antiClownApiClient.Inventories.ReadInventoryAsync(userId);
        var items = CollectItems(inventory);

        var id = Guid.NewGuid();
        var inventoryDetails = new InventoryDetails
        {
            UserId = userId,
            CurrentPage = 0,
            Tool = InventoryTool.ChangeActiveStatus,
            Pages = BuildPages(items),
        };
        var embed = await inventoryEmbedBuilder.BuildAsync(inventoryDetails, items);

        var message = await createMessage(context, await BuildWebhookBuilderAsync(id, embed, addButtons: inventoryDetails.Pages.Length > 0));

        var inventoryInteractivity = new Interactivity<InventoryDetails>
        {
            Id = id,
            Type = InteractivityType.Inventory,
            AuthorId = context.Member.Id,
            MessageId = message.Id,
            Details = inventoryDetails,
        };
        await interactivityRepository.CreateAsync(inventoryInteractivity);
    }

    public async Task HandleItemInSlotAsync(Guid inventoryId, int slot, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<InventoryDetails>(inventoryId);
        if (interactivity is null)
        {
            return;
        }

        var inventoryDetails = interactivity.Details!;
        if (inventoryDetails.Pages.Length == 0)
        {
            return;
        }

        var currentPage = inventoryDetails.Pages[inventoryDetails.CurrentPage];
        if (slot >= currentPage.ItemsIdsOnPage.Length)
        {
            return;
        }

        var itemId = currentPage.ItemsIdsOnPage[slot];
        var errorMessage = inventoryDetails.Tool switch
        {
            InventoryTool.Sell => await SellItemAsync(inventoryDetails.UserId, itemId),
            InventoryTool.ChangeActiveStatus => await ChangeActiveStatusOfItemAsync(inventoryDetails.UserId, itemId),
            _ => throw new ArgumentOutOfRangeException(),
        };
        var updatedInventory = await antiClownApiClient.Inventories.ReadInventoryAsync(inventoryDetails.UserId);
        var items = CollectItems(updatedInventory);
        inventoryDetails.Pages = BuildPages(items);
        inventoryDetails.CurrentPage = Math.Min(inventoryDetails.CurrentPage, inventoryDetails.Pages.Length - 1);
        await interactivityRepository.UpdateAsync(interactivity);
        var embed = await inventoryEmbedBuilder.BuildAsync(inventoryDetails, items);
        await updateMessage(await BuildWebhookBuilderAsync(inventoryId, embed, errorMessage, inventoryDetails.Pages.Length > 0));
    }

    public async Task SetActiveToolAsync(Guid inventoryId, InventoryTool tool, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<InventoryDetails>(inventoryId);
        if (interactivity is null)
        {
            return;
        }

        interactivity.Details!.Tool = tool;
        await interactivityRepository.UpdateAsync(interactivity);
        var inventory = await antiClownApiClient.Inventories.ReadInventoryAsync(interactivity.Details.UserId);
        var items = CollectItems(inventory);
        var embed = await inventoryEmbedBuilder.BuildAsync(interactivity.Details, items);
        await updateMessage(await BuildWebhookBuilderAsync(inventoryId, embed, addButtons: interactivity.Details.Pages.Length > 0));
    }

    public async Task ChangePageAsync(Guid inventoryId, int pageDiff, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<InventoryDetails>(inventoryId);
        if (interactivity is null)
        {
            return;
        }

        interactivity.Details!.CurrentPage = (interactivity.Details!.CurrentPage + pageDiff) % interactivity.Details!.Pages.Length;
        await interactivityRepository.UpdateAsync(interactivity);
        var inventory = await antiClownApiClient.Inventories.ReadInventoryAsync(interactivity.Details.UserId);
        var items = CollectItems(inventory);
        var embed = await inventoryEmbedBuilder.BuildAsync(interactivity.Details, items);
        await updateMessage(await BuildWebhookBuilderAsync(inventoryId, embed, addButtons: interactivity.Details.Pages.Length > 0));
    }

    private async Task<DiscordWebhookBuilder> BuildWebhookBuilderAsync(Guid id, DiscordEmbed embed, string? message = "", bool addButtons = false)
    {
        var webhookBuilder = new DiscordWebhookBuilder().AddEmbed(embed);
        if (message is not null)
        {
            webhookBuilder = webhookBuilder.WithContent(message);
        }

        if (addButtons)
        {
            webhookBuilder = webhookBuilder.AddComponents(
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Primary, InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButton1), null,
                                                   false, new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("one"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Primary, InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButton2), null,
                                                   false, new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("two"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Primary, InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButton3), null,
                                                   false, new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("three"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Primary, InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButton4), null,
                                                   false, new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("four"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Primary, InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButton5), null,
                                                   false, new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("five"))
                                               )
                                           )
                                           .AddComponents(
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Success,
                                                   InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButtonLeft),
                                                   null,
                                                   false,
                                                   new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("arrow_left"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Success,
                                                   InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButtonRight),
                                                   null,
                                                   false,
                                                   new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("arrow_right"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Secondary,
                                                   InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButtonChangeActiveStatus),
                                                   null,
                                                   false,
                                                   new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("repeat"))
                                               ),
                                               new DiscordButtonComponent(
                                                   ButtonStyle.Secondary,
                                                   InteractionsIds.InventoryButtons.BuildId(id, InteractionsIds.InventoryButtons.InventoryButtonSell),
                                                   null,
                                                   false,
                                                   new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("x"))
                                               )
                                           );
        }

        return webhookBuilder;
    }

    private async Task<string> ChangeActiveStatusOfItemAsync(Guid ownerId, Guid itemId)
    {
        try
        {
            var item = await antiClownApiClient.Inventories.ReadItemAsync(ownerId, itemId);
            await antiClownApiClient.Inventories.ChangeItemActiveStatusAsync(ownerId, itemId, !item.IsActive);
            return "";
        }
        catch (ForbiddenInactiveStatusForNegativeItemException)
        {
            return "Негативный предмет нельзя сделать неактивным!";
        }
        catch (TooManyActiveItemsCountException)
        {
            return "Невозможно изменить статус предмета, активных предметов должно быть не более 3!";
        }
    }

    private async Task<string> SellItemAsync(Guid ownerId, Guid itemId)
    {
        try
        {
            await antiClownApiClient.Inventories.SellAsync(ownerId, itemId);
            return "";
        }
        catch (NotEnoughBalanceException)
        {
            return "Недостаточно денег для продажи негативного предмета";
        }
    }

    private static InventoryPage[] BuildPages(BaseItemDto[] items)
    {
        var pages = new List<InventoryPage>();
        var itemsGroupsByName = items.GroupBy(item => item.ItemName);
        foreach (var nameGroup in itemsGroupsByName)
        {
            var groupItems = nameGroup.ToList();
            var chunkItems = groupItems.Chunk(ItemsPerPage);
            var offset = 0;
            chunkItems.ForEach(
                pageItems =>
                {
                    var currentOffset = offset;
                    pages.Add(
                        new InventoryPage
                        {
                            ItemName = nameGroup.Key,
                            PageDescription = $"Предметы {currentOffset + 1}-{currentOffset + pageItems.Length} из {groupItems.Count}",
                            ItemsIdsOnPage = pageItems.Select(x => x.Id).ToArray(),
                        }
                    );
                    offset += pageItems.Length;
                }
            );
        }

        return pages.ToArray();
    }

    private static BaseItemDto[] CollectItems(InventoryDto inventory)
    {
        // kinda bruh
        return (inventory.CatWives as BaseItemDto[])
               .Concat(inventory.CommunismBanners)
               .Concat(inventory.DogWives)
               .Concat(inventory.Internets)
               .Concat(inventory.JadeRods)
               .Concat(inventory.RiceBowls)
               .ToArray();
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IInventoryEmbedBuilder inventoryEmbedBuilder;
    private readonly IUsersCache usersCache;

    private const int ItemsPerPage = 5;
}