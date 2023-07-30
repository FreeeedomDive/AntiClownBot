using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Services.Shop;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Inventory;

[SlashCommandGroup(InteractionsIds.CommandsNames.Shop_Group, "Команды, связанные с магазином")]
public class ShopCommandModule : SlashCommandModuleWithMiddlewares
{
    public ShopCommandModule(
        ICommandExecutor commandExecutor,
        IShopService shopService,
        IUsersCache usersCache,
        IAntiClownApiClient antiClownApiClient
    ) : base(commandExecutor)
    {
        this.shopService = shopService;
        this.usersCache = usersCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Shop_Open, "Открыть магазин")]
    public async Task GetShop(InteractionContext context)
    {
        await ExecuteAsync(context, async () => await shopService.CreateAsync(context, RespondToInteractionAsync));
    }

    [SlashCommand(InteractionsIds.CommandsNames.Shop_Stats, "Посмотреть статистику")]
    public async Task ShopShopStats(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var shopStats = await antiClownApiClient.Shops.ReadStatsAsync(userId);
                var embed = new DiscordEmbedBuilder()
                            .WithTitle($"Статистика магазина {context.Member.ServerOrUserName()}")
                            .WithColor(context.Member.Color)
                            .AddField(
                                "Покупки", 
                                $"Предметов куплено: {shopStats.ItemsBought}\nДенег потрачено: {shopStats.ScamCoinsLostOnPurchases}")
                            .AddField(
                                "Рероллы",
                                $"Рероллов сделано: {shopStats.TotalReRolls}\nДенег потрачено: {shopStats.ScamCoinsLostOnReRolls}")
                            .AddField(
                                "Распознавания",
                                $"Предметов распознано: {shopStats.TotalReveals}\nДенег потрачено: {shopStats.ScamCoinsLostOnReveals}")
                            .Build();
                await RespondToInteractionAsync(context, embed);
            }
        );
    }

    private readonly IShopService shopService;
    private readonly IUsersCache usersCache;
    private readonly IAntiClownApiClient antiClownApiClient;
}