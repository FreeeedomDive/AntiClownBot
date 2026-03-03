using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating
{
    public class LohotronCommandModule : SlashCommandModuleWithMiddlewares
    {
        public LohotronCommandModule(
            ICommandExecutor commandExecutor,
            IEmotesCache emotesCache,
            IAntiClownApiClient antiClownApiClient,
            IUsersCache usersCache
        ) : base(commandExecutor)
        {
            this.emotesCache = emotesCache;
            this.antiClownApiClient = antiClownApiClient;
            this.usersCache = usersCache;
        }

        [SlashCommand(InteractionsIds.CommandsNames.Lohotron, "Прокрутить колесо лохотрона (1 раз в день)")]
        public async Task PlayLohotron(InteractionContext context)
        {
            await ExecuteAsync(context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                try
                {
                    var lohotronPrize = await antiClownApiClient.Lohotron.UseLohotronAsync(userId);
                    switch (lohotronPrize.RewardType)
                    {
                        case LohotronRewardTypeDto.Nothing:
                            var peepoFinger = await emotesCache.GetEmoteAsTextAsync("peepoFinger");
                            await RespondToInteractionAsync(context, $"Ты получаешь {peepoFinger}!");
                            return;
                        case LohotronRewardTypeDto.ScamCoins:
                            await RespondToInteractionAsync(context, $"Ты получаешь {lohotronPrize.ScamCoinsReward!.Value} scam coins");
                            return;
                        case LohotronRewardTypeDto.LootBox:
                            await RespondToInteractionAsync(context, "Ты получаешь добычу-коробку!");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (LohotronAlreadyUsedException)
                {
                    var peepoFinger = await emotesCache.GetEmoteAsTextAsync("peepoFinger");
                    await RespondToInteractionAsync(context, $"Чел, 2 раза нельзя! {peepoFinger}");
                }
            });
        }

        private readonly IEmotesCache emotesCache;
        private readonly IAntiClownApiClient antiClownApiClient;
        private readonly IUsersCache usersCache;
    }
}
