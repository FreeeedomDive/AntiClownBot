using System;
using System.Linq;
using System.Threading.Tasks;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses.UserCommandResponses;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class TributeCommand : BaseCommand
    {
        public TributeCommand(DiscordClient discord, Configuration configuration) : base(
            discord, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            if (e.Channel.Id != 877994939240292442 && e.Channel.Id != 879784704696549498)
            {
                await e.Message.RespondAsync($"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(877994939240292442).Mention}");
                return;
            }
            
            var tributeResult = ApiWrapper.Wrappers.UsersWrapper.Tribute(e.Author.Id);
            var embed = MakeEmbedForTribute(tributeResult);
            await e.Message.RespondAsync(embed);
        }

        public static DiscordEmbed MakeEmbedForTribute(TributeResponseDto response)
        {
            var member = Configuration.GetServerMember(response.UserId);
            var messageEmbedBuilder = new DiscordEmbedBuilder();
            messageEmbedBuilder.WithTitle($"Подношение {member.Nickname}");
            
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
                        var item = UsersWrapper.GetItemById(response.UserId, baseItem).Item;
                        var percent = item switch
                        {
                            Internet internet => internet.Ping,
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

        public override string Help()
        {
            return
                "Преподношение императору XI для увеличения (или уменьшения) своего баланса скам койнов\nДефолтный кулдаун 1 час, понижается наличием интернета";
        }
    }
}