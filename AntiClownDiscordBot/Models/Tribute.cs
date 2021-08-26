using System.Linq;
using AntiClownBot.Helpers;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses.UserCommandResponses;
using ApiWrapper.Wrappers;
using DSharpPlus.Entities;

namespace AntiClownBot.Models
{
    public class Tribute
    {
        public static DiscordEmbed MakeEmbedForTribute(TributeResponseDto response)
        {
            var member = Configuration.GetServerMember(response.UserId);
            var messageEmbedBuilder = new DiscordEmbedBuilder();
            var tributeTitle = response.IsTributeAutomatic ? "Подношение кошки-жены" : "Подношение";
            messageEmbedBuilder.WithTitle($"{tributeTitle} {member.ServerOrUserName()}");
            
            if (response.Result == TributeResult.CooldownHasNotPassed)
            {
                messageEmbedBuilder.WithColor(DiscordColor.Red);
                messageEmbedBuilder.AddField(Utility.StringEmoji(":NOPERS:"),
                    $"Не злоупотребляй подношение император XI {Utility.StringEmoji(":PepegaGun:")}");
                return messageEmbedBuilder.Build();
            }

            if (response.IsCommunismActive)
            {
                var sharedUser = Configuration.GetServerMember(response.SharedCommunistUserId);
                messageEmbedBuilder.AddField($"Произошел коммунизм {Utility.StringEmoji(":cykaPls:")}",
                    $"Разделение подношения с {sharedUser.Nickname}");
            }
            
            switch (response.TributeQuality)
            {
                case > 0:
                    messageEmbedBuilder.WithColor(DiscordColor.Green);
                    messageEmbedBuilder.AddField($"+{response.TributeQuality} scam coins", 
                        "Партия гордится вами!!!");
                    break;
                case < 0:
                    messageEmbedBuilder.WithColor(DiscordColor.Red);
                    messageEmbedBuilder.AddField($"{response.TributeQuality} scam coins", 
                        "Ну и ну! Вы разочаровать партию!");
                    break;
                default:
                    messageEmbedBuilder.WithColor(DiscordColor.White);
                    messageEmbedBuilder.AddField("Партия не оценить ваших усилий",
                        $"{Utility.StringEmoji(":Starege:")} {Utility.StringEmoji(":Starege:")} {Utility.StringEmoji(":Starege:")}");
                    break;
            }

            if (response.CooldownModifiers.Count > 0)
            {
                var modifiers = response
                    .CooldownModifiers
                    .Select(kv =>
                    {
                        var (baseItem, count) = kv;
                        var item = UsersApi.GetItemById(response.UserId, baseItem).Item;
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
                messageEmbedBuilder.AddField(
                    $"{Utility.StringEmoji(":RainbowPls:")} Кошка-жена {Utility.StringEmoji(":RainbowPls:")}",
                    $"Кошка-жена подарить тебе автоматический следующий подношение {Utility.StringEmoji(":Pog:")}");
            }

            return messageEmbedBuilder.Build();
        }

    }
}