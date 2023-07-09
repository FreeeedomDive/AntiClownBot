using AntiClownApiClient;
using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models
{
    public class TributeService
    {
        public TributeService(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IApiClient apiClient,
            IUserBalanceService userBalanceService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.apiClient = apiClient;
            this.userBalanceService = userBalanceService;
        }

        public async Task<DiscordEmbed?> TryMakeEmbedForTribute(TributeResponseDto response)
        {
            var member = await discordClientWrapper.Members.GetAsync(response.UserId);
            var messageEmbedBuilder = new DiscordEmbedBuilder();
            var tributeTitle = response.IsTributeAutomatic ? "Подношение кошки-жены" : "Подношение";
            messageEmbedBuilder.WithTitle($"{tributeTitle} {member.ServerOrUserName()}");
            messageEmbedBuilder.WithColor(member.Color);

            switch (response.Result)
            {
                case TributeResult.Success:
                    break;
                case TributeResult.AutoTributeWasCancelledByEarlierTribute:
                    return null;
                case TributeResult.CooldownHasNotPassed:
                    messageEmbedBuilder.AddField(await emotesProvider.GetEmoteAsTextAsync("NOPERS"),
                        "Не злоупотребляй подношение император XI"
                        + $" {await emotesProvider.GetEmoteAsTextAsync("PepegaGun")}");
                    return messageEmbedBuilder.Build();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            messageEmbedBuilder.WithThumbnail(member.AvatarUrl);

            if (response.IsCommunismActive)
            {
                var sharedUser = await discordClientWrapper.Members.GetAsync(response.SharedCommunistUserId);
                messageEmbedBuilder.AddField(
                    $"Произошел коммунизм {await emotesProvider.GetEmoteAsTextAsync("cykaPls")}",
                    $"Разделение подношения с {sharedUser.ServerOrUserName()}"
                );
                userBalanceService.ChangeDailyStats(response.SharedCommunistUserId, response.TributeQuality);
            }

            userBalanceService.ChangeDailyStats(response.UserId, response.TributeQuality);
            switch (response.TributeQuality)
            {
                case > 0:
                    messageEmbedBuilder.AddField($"+{response.TributeQuality} scam coins",
                        "Партия гордится вами!!!");
                    break;
                case < 0:
                    messageEmbedBuilder.AddField($"{response.TributeQuality} scam coins",
                        "Ну и ну! Вы разочаровать партию!");
                    break;
                default:
                    var starege = await emotesProvider.GetEmoteAsTextAsync("Starege");
                    messageEmbedBuilder.AddField("Партия не оценить ваших усилий",
                        $"{starege} {starege} {starege}");
                    break;
            }

            if (response.CooldownModifiers.Count > 0)
            {
                var modifiers = response
                    .CooldownModifiers
                    .Select(kv =>
                    {
                        var (baseItem, count) = kv;
                        var item = apiClient.Users.GetItemByIdAsync(response.UserId, baseItem)
                            .GetAwaiter().GetResult()
                            .Item;
                        var percent = item switch
                        {
                            Internet internet => internet.Speed,
                            JadeRod jadeRod => jadeRod.Thickness,
                            _ => 0
                        };

                        var sign = item.ItemType == ItemType.Positive ? "-" : "+";
                        return $"{item.Rarity} {item.Name} ({sign}{percent}%) : x{count}";
                    });

                messageEmbedBuilder.AddField($"Модификаторы кулдауна",
                    string.Join("\n", modifiers));
            }

            if (response.IsNextTributeAutomatic)
            {
                var rainbowEmote = await emotesProvider.GetEmoteAsTextAsync("RainbowPls");
                messageEmbedBuilder.AddField(
                    $"{rainbowEmote} Кошка-жена {rainbowEmote}",
                    "Кошка-жена подарить тебе автоматический следующий подношение "
                    + $"{await emotesProvider.GetEmoteAsTextAsync("Pog")}"
                );
            }

            if (response.HasLootBox)
            {
                messageEmbedBuilder.AddField(
                    "Лутбокс",
                    "Император подарил вам добычу-коробку");
            }

            return messageEmbedBuilder.Build();
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IApiClient apiClient;
        private readonly IUserBalanceService userBalanceService;
    }
}