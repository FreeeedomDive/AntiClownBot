using AntiClown.Api.Client;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Shops;
using AntiClown.Api.Dto.Shops;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Shops;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.Shop;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.Interactivity.Services.Shop;

public class ShopService : IShopService
{
    public ShopService(
        IAntiClownApiClient antiClownApiClient,
        IShopEmbedBuilder shopEmbedBuilder,
        IInteractivityRepository interactivityRepository,
        IUsersCache usersCache,
        IEmotesCache emotesCache
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.shopEmbedBuilder = shopEmbedBuilder;
        this.interactivityRepository = interactivityRepository;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
    }

    public async Task CreateAsync(InteractionContext context, Func<InteractionContext, DiscordWebhookBuilder, Task<DiscordMessage>> createMessage)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
        var shop = await antiClownApiClient.Shops.ReadAsync(userId);

        var id = Guid.NewGuid();
        var shopDetails = new ShopDetails
        {
            UserId = userId,
            Tool = ShopTool.Buying,
            BoughtItems = new Dictionary<int, Guid>(),
            Shop = shop,
        };

        var embed = await shopEmbedBuilder.BuildAsync(shopDetails);
        var message = await createMessage(context, await BuildWebhookBuilderAsync(id, embed, shopDetails));

        var shopInteractivity = new Interactivity<ShopDetails>
        {
            Id = id,
            Type = InteractivityType.Shop,
            AuthorId = context.Member.Id,
            MessageId = message.Id,
            Details = shopDetails,
        };
        await interactivityRepository.CreateAsync(shopInteractivity);
    }

    public async Task HandleItemInSlotAsync(Guid shopId, int slot, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<ShopDetails>(shopId);
        if (interactivity is null)
        {
            return;
        }

        var shopDetails = interactivity.Details!;
        if (slot >= shopDetails.Shop.Items.Length)
        {
            return;
        }

        var shopItemId = shopDetails.Shop.Items[slot].Id;
        var errorMessage = shopDetails.Tool switch
        {
            ShopTool.Buying => await BuyAsync(shopDetails.UserId, shopItemId, slot, shopDetails),
            ShopTool.Revealing => await RevealAsync(shopDetails.UserId, shopItemId),
            _ => throw new ArgumentOutOfRangeException(),
        };
        var updatedShop = await antiClownApiClient.Shops.ReadAsync(shopDetails.UserId);
        shopDetails.Shop = updatedShop;
        await interactivityRepository.UpdateAsync(interactivity);
        var embed = await shopEmbedBuilder.BuildAsync(shopDetails);
        await updateMessage(await BuildWebhookBuilderAsync(shopId, embed, shopDetails, errorMessage));
    }

    public async Task SetActiveToolAsync(Guid shopId, ShopTool tool, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<ShopDetails>(shopId);
        if (interactivity is null)
        {
            return;
        }

        interactivity.Details!.Tool = tool;
        await interactivityRepository.UpdateAsync(interactivity);
        var embed = await shopEmbedBuilder.BuildAsync(interactivity.Details);
        await updateMessage(await BuildWebhookBuilderAsync(shopId, embed, interactivity.Details));
    }

    public async Task ReRollAsync(Guid shopId, Func<DiscordWebhookBuilder, Task> updateMessage)
    {
        var interactivity = await interactivityRepository.TryReadAsync<ShopDetails>(shopId);
        if (interactivity is null)
        {
            return;
        }

        string? errorMessage = null;
        var shopDetails = interactivity.Details!;
        try
        {
            await antiClownApiClient.Shops.ReRollShopAsync(shopDetails.UserId);
            var updatedShop = await antiClownApiClient.Shops.ReadAsync(shopDetails.UserId);
            shopDetails.Shop = updatedShop;
            shopDetails.BoughtItems = new Dictionary<int, Guid>();
            await interactivityRepository.UpdateAsync(interactivity);
        }
        catch (NotEnoughBalanceException e)
        {
            errorMessage = $"Не хватает {e.OperationCost - e.UserBalance} скам-койнов для покупки";
        }

        var embed = await shopEmbedBuilder.BuildAsync(shopDetails);
        await updateMessage(await BuildWebhookBuilderAsync(shopId, embed, shopDetails, errorMessage));
    }

    private async Task<string?> BuyAsync(Guid userId, Guid itemId, int slot, ShopDetails shopDetails)
    {
        try
        {
            var shopItem = await antiClownApiClient.Shops.BuyItemAsync(userId, itemId);
            shopDetails.BoughtItems[slot] = shopItem.Id;
            return null;
        }
        catch (ShopItemNotFoundException)
        {
            return "Предмет не найден в магазине";
        }
        catch (ShopItemAlreadyBoughtException)
        {
            return "Предмет уже куплен";
        }
        catch (NotEnoughBalanceException e)
        {
            return $"Не хватает {e.OperationCost - e.UserBalance} скам-койнов для покупки";
        }
    }

    private async Task<string?> RevealAsync(Guid userId, Guid itemId)
    {
        try
        {
            await antiClownApiClient.Shops.RevealItemAsync(userId, itemId);
            return null;
        }
        catch (ShopItemNotFoundException)
        {
            return "Предмет не найден в магазине";
        }
        catch (ShopItemAlreadyBoughtException)
        {
            return "Предмет уже куплен";
        }
        catch (NotEnoughBalanceException e)
        {
            return $"Не хватает {e.OperationCost - e.UserBalance} скам-койнов для покупки";
        }
    }

    private async Task<DiscordWebhookBuilder> BuildWebhookBuilderAsync(Guid id, DiscordEmbed embed, ShopDetails shopDetails, string? message = "")
    {
        var webhookBuilder = new DiscordWebhookBuilder().AddEmbed(embed);
        if (message is not null)
        {
            webhookBuilder = webhookBuilder.WithContent(message);
        }

        var revealing = shopDetails.Tool == ShopTool.Revealing;
        webhookBuilder.AddComponents(
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Success,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonItem1),
                              null,
                              IsButtonDisabled(revealing, shopDetails.Shop.Items[0]),
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("one"))
                          ),
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Success,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonItem2),
                              null,
                              IsButtonDisabled(revealing, shopDetails.Shop.Items[1]),
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("two"))
                          ),
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Success,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonItem3),
                              null,
                              IsButtonDisabled(revealing, shopDetails.Shop.Items[2]),
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("three"))
                          ),
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Success,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonItem4),
                              null,
                              IsButtonDisabled(revealing, shopDetails.Shop.Items[3]),
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("four"))
                          ),
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Success,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonItem5),
                              null,
                              IsButtonDisabled(revealing, shopDetails.Shop.Items[4]),
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("five"))
                          )
                      )
                      .AddComponents(
                          new DiscordButtonComponent(
                              !revealing ? ButtonStyle.Success : ButtonStyle.Secondary,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonBuy),
                              "Купить",
                              false,
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("PepegaCredit"))
                          ),
                          new DiscordButtonComponent(
                              revealing ? ButtonStyle.Primary : ButtonStyle.Secondary,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonReveal),
                              $"Распознать ({(shopDetails.Shop.FreeReveals > 0 ? "бесплатно" : " 40% стоимости")})",
                              false,
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("pepeSearching"))
                          )
                      )
                      .AddComponents(
                          new DiscordButtonComponent(
                              ButtonStyle.Danger,
                              InteractionsIds.ShopButtons.BuildId(id, InteractionsIds.ShopButtons.ShopButtonReroll),
                              $"Обновить предметы в магазине (-{shopDetails.Shop.ReRollPrice})",
                              false,
                              new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("COGGERS"))
                          )
                      );

        return webhookBuilder;
    }

    private static bool IsButtonDisabled(bool revealing, ShopItemDto item)
    {
        return item.IsOwned || (revealing && item.IsRevealed);
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IShopEmbedBuilder shopEmbedBuilder;
    private readonly IUsersCache usersCache;
}