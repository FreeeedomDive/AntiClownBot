using System.Diagnostics;
using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Tributes;

public class TributeEmbedBuilder : ITributeEmbedBuilder
{
    public TributeEmbedBuilder(
        IAntiClownApiClient antiClownApiClient,
        IEmotesCache emotesCache,
        IUsersCache usersCache
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
    }

    public async Task<DiscordEmbed> BuildForSuccessfulTributeAsync(TributeDto tribute)
    {
        var messageEmbedBuilder = new DiscordEmbedBuilder();
        var member = await usersCache.GetMemberByApiIdAsync(tribute.UserId);
        var tributeTitle = tribute.IsAutomatic ? "Подношение кошки-жены" : "Подношение";
        messageEmbedBuilder.WithTitle($"{tributeTitle} {member.ServerOrUserName()}");
        if (member is not null)
        {
            messageEmbedBuilder.WithColor(member.Color);
            messageEmbedBuilder.WithThumbnail(member.AvatarUrl);
        }

        if (tribute.SharedUserId is not null)
        {
            var sharedMember = await usersCache.GetMemberByApiIdAsync(tribute.SharedUserId.Value);
            var cykaPls = await emotesCache.GetEmoteAsTextAsync("cykaPls");
            messageEmbedBuilder.AddField(
                $"Произошел коммунизм {cykaPls}",
                $"Разделение подношения с {sharedMember.ServerOrUserName()}"
            );
        }

        switch (tribute.ScamCoins)
        {
            case > 0:
                messageEmbedBuilder.AddField(
                    $"+{tribute.ScamCoins} scam coins",
                    "Партия гордится вами!!!"
                );
                break;
            case < 0:
                messageEmbedBuilder.AddField(
                    $"{tribute.ScamCoins} scam coins",
                    "Ну и ну! Вы разочаровать партию!"
                );
                break;
            default:
                var starege = await emotesCache.GetEmoteAsTextAsync("Starege");
                messageEmbedBuilder.AddField(
                    "Партия не оценить ваших усилий",
                    $"{starege} {starege} {starege}"
                );
                break;
        }

        if (tribute.CooldownModifiers.Count > 0)
        {
            var modifiersTasks = tribute
                                 .CooldownModifiers
                                 .Select(
                                     async kv =>
                                     {
                                         var (itemId, count) = kv;
                                         var item = await antiClownApiClient.Inventories.ReadItemAsync(tribute.UserId, itemId);
                                         var percent = item switch
                                         {
                                             CatWifeDto => throw new UnreachableException($"{nameof(CatWifeDto)} can not be cooldown modifier"),
                                             CommunismBannerDto => throw new UnreachableException($"{nameof(CommunismBannerDto)} can not be cooldown modifier"),
                                             DogWifeDto => throw new UnreachableException($"{nameof(DogWifeDto)} can not be cooldown modifier"),
                                             InternetDto internet => internet.Speed,
                                             JadeRodDto jadeRod => jadeRod.Thickness,
                                             RiceBowlDto => throw new UnreachableException($"{nameof(RiceBowlDto)} can not be cooldown modifier"),
                                             _ => throw new ArgumentOutOfRangeException(),
                                         };

                                         var sign = item.ItemType == ItemTypeDto.Positive ? "-" : "+";
                                         return $"{item.Rarity} {item.ItemName.Localize()} ({sign}{percent}%) : x{count}";
                                     }
                                 );

            var modifiers = await Task.WhenAll(modifiersTasks);

            messageEmbedBuilder.AddField("Модификаторы кулдауна", string.Join("\n", modifiers));
        }

        if (tribute.IsNextAutomatic)
        {
            var rainbowPls = await emotesCache.GetEmoteAsTextAsync("RainbowPls");
            var pog = await emotesCache.GetEmoteAsTextAsync("Pog");
            messageEmbedBuilder.AddField(
                $"{rainbowPls} Кошка-жена {rainbowPls}",
                $"Кошка-жена подарить тебе автоматический следующий подношение {pog}"
            );
        }

        if (tribute.HasGiftedLootBox)
        {
            messageEmbedBuilder.AddField(
                "Лутбокс",
                "Император подарил вам добычу-коробку"
            );
        }

        messageEmbedBuilder.WithFooter($"Время подношения: {tribute.TributeDateTime.ToYekaterinburgTime()}");

        return messageEmbedBuilder.Build();
    }

    public async Task<DiscordEmbed> BuildForTributeOnCoolDownAsync()
    {
        var messageEmbedBuilder = new DiscordEmbedBuilder();

        messageEmbedBuilder.WithColor(DiscordColor.Red);

        var nopers = await emotesCache.GetEmoteAsTextAsync("NOPERS");
        var pepegaGun = await emotesCache.GetEmoteAsTextAsync("PepegaGun");
        messageEmbedBuilder.AddField(nopers, $"Не злоупотребляй подношение император XI {pepegaGun}");

        return messageEmbedBuilder.Build();
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}