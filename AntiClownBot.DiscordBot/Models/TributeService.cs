using AntiClownApiClient;
using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models
{
    public class TributeService
    {
        public TributeService(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IUserBalanceService userBalanceService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
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
                    messageEmbedBuilder.AddField(await discordClientWrapper.Emotes.FindEmoteAsync("NOPERS"),
                        "Не злоупотребляй подношение император XI"
                        + $" {await discordClientWrapper.Emotes.FindEmoteAsync("PepegaGun")}");
                    return messageEmbedBuilder.Build();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            messageEmbedBuilder.WithThumbnail(member.AvatarUrl);

            if (response.IsCommunismActive)
            {
                var sharedUser = await discordClientWrapper.Members.GetAsync(response.SharedCommunistUserId);
                messageEmbedBuilder.AddField(
                    $"Произошел коммунизм {await discordClientWrapper.Emotes.FindEmoteAsync("cykaPls")}",
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
                    var starege = await discordClientWrapper.Emotes.FindEmoteAsync("Starege");
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
                var rainbowEmote = await discordClientWrapper.Emotes.FindEmoteAsync("RainbowPls");
                messageEmbedBuilder.AddField(
                    $"{rainbowEmote} Кошка-жена {rainbowEmote}",
                    "Кошка-жена подарить тебе автоматический следующий подношение "
                    + $"{await discordClientWrapper.Emotes.FindEmoteAsync("Pog")}"
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
        private readonly IApiClient apiClient;
        private readonly IUserBalanceService userBalanceService;
    }
}