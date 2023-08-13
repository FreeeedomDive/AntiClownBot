using AntiClown.Api.Client;
using AntiClown.Api.Dto.Exceptions.Items;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Inventories;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Inventory;

public class LootBoxCommandModule : SlashCommandModuleWithMiddlewares
{
    public LootBoxCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        ILootBoxEmbedBuilder lootBoxEmbedBuilder,
        IUsersCache usersCache,
        IEmotesCache emotesCache
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.lootBoxEmbedBuilder = lootBoxEmbedBuilder;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.LootBox, "Открыть лутбокс")]
    public async Task OpenLootBox(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                try
                {
                    var member = context.Member;
                    var userId = await usersCache.GetApiIdByMemberIdAsync(member.Id);
                    var lootBoxReward = await antiClownApiClient.Inventories.OpenLootBoxAsync(userId);

                    var embed = await lootBoxEmbedBuilder.BuildAsync(userId, lootBoxReward);

                    await RespondToInteractionAsync(context, embed);
                }
                catch (LootBoxNotFoundException)
                {
                    await RespondToInteractionAsync(
                        context,
                        $"Нет доступных лутбоксов {await emotesCache.GetEmoteAsTextAsync("modCheck")}"
                    );
                }
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEmotesCache emotesCache;
    private readonly ILootBoxEmbedBuilder lootBoxEmbedBuilder;
    private readonly IUsersCache usersCache;
}