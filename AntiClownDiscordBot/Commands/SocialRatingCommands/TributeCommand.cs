using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public override void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            Tribute(e, user, false);
        }

        private async void Tribute(MessageCreateEventArgs e, SocialRatingUser user, bool isAutomatic)
        {
            var messageEmbedBuilder = new DiscordEmbedBuilder();
            messageEmbedBuilder.WithTitle(isAutomatic
                ? $"Автоматическое подношение для {user.DiscordUsername}"
                : "Подношение");
            
            if (!Config.AreTributesOpen)
            {
                messageEmbedBuilder.WithColor(DiscordColor.Red);
                messageEmbedBuilder.AddField($"Я ЗАНЯТ!!! {Utility.Emoji(":NOPERS:")}", isAutomatic
                    ? "Приводи свою кошку-жену позже, а щас отъебись"
                    : "Отъебись");
                await e.Message.RespondAsync(messageEmbedBuilder.Build());
                return;
            }
            
            if (!user.IsCooldownPassed())
            {
                if (isAutomatic)
                {
                    messageEmbedBuilder.WithColor(DiscordColor.Red);
                    messageEmbedBuilder.AddField(Utility.StringEmoji(":NOPERS:"),
                        "Ты успеть принести подношение до твой кошачья жена, подношение от кошки не учитываться");
                    await e.Message.RespondAsync(messageEmbedBuilder.Build());
                    return;
                }

                messageEmbedBuilder.WithColor(DiscordColor.Red);
                messageEmbedBuilder.AddField(Utility.StringEmoji(":NOPERS:"),
                    $"Не злоупотребляй подношение император XI {Utility.StringEmoji(":PepegaGun:")}");
                await e.Message.RespondAsync(messageEmbedBuilder.Build());
                user.ChangeRating(-15);
                return;
            }

            var (gigabyteWorked, jadeRodWorked) = user.UpdateCooldown();

            var tributeQuality = Randomizer.GetRandomNumberBetween(
                Constants.MinTributeValue -
                user.Stats.TributeLowerExtendBorder,
                Constants.MaxTributeValue +
                user.Stats.TributeUpperExtendBorder);
            var communism = Randomizer.GetRandomNumberBetween(0, 100) < user.Stats.TributeSplitChance;

            var sharedUser = user;
            if (communism)
            {
                sharedUser = Utility
                    .GetDistributedCommunists()
                    .Where(u => u.DiscordId != user.DiscordId)
                    .SelectRandomItem();
                messageEmbedBuilder.AddField($"Произошел коммунизм {Utility.StringEmoji(":cykaPls:")}",
                    $"Разделение подношения с {sharedUser.DiscordUsername}");
                tributeQuality /= 2;
            }

            switch (tributeQuality)
            {
                case > 0:
                    messageEmbedBuilder.WithColor(DiscordColor.Green);
                    messageEmbedBuilder.AddField($"+{tributeQuality} social credit", 
                        "Партия гордится вами!!!");
                    break;
                case < 0:
                    messageEmbedBuilder.WithColor(DiscordColor.Red);
                    messageEmbedBuilder.AddField($"-{-tributeQuality} social credit", 
                        "Ну и ну! Вы разочаровать партию!");
                    break;
                default:
                    messageEmbedBuilder.WithColor(DiscordColor.White);
                    messageEmbedBuilder.AddField("Партия не оценить ваших усилий",
                        $"{Utility.StringEmoji(":Starege:")} {Utility.StringEmoji(":Starege:")} {Utility.StringEmoji(":Starege:")}");
                    break;
            }

            if (gigabyteWorked != 0 || jadeRodWorked != 0)
            {
                var changeString = "";
                changeString += gigabyteWorked > 0 ? $"гигабайт интернет x{gigabyteWorked}" : "";
                changeString += gigabyteWorked > 0 && jadeRodWorked > 0 ? " и " : "";
                changeString += jadeRodWorked > 0 ? $"нефритовый стержень x{jadeRodWorked}" : "";
                messageEmbedBuilder.AddField("Изменение кулдауна", changeString);
            }

            var isNextTributeAutomatic = Randomizer.GetRandomNumberBetween(0, 100) < user.Stats.TributeAutoChance;
            if (isNextTributeAutomatic)
            {
                messageEmbedBuilder.AddField(
                    $"{Utility.StringEmoji(":RainbowPls:")} Кошка-жена {Utility.StringEmoji(":RainbowPls:")}",
                    $"Кошка-жена подарить тебе автоматический следующий подношение {Utility.StringEmoji(":Pog:")}");
            }

            await e.Message.RespondAsync(messageEmbedBuilder.Build());
            user.ChangeRating(tributeQuality);
            if (communism)
            {
                sharedUser.ChangeRating(tributeQuality);
            }

            if (!isNextTributeAutomatic) return;

            var thread = new Thread(async () =>
            {
                await Task.Delay((int) (user.NextTribute - DateTime.Now).TotalMilliseconds + 1000);
                Tribute(e, user, true);
            })
            {
                IsBackground = true
            };
            thread.Start();
        }

        public override string Help()
        {
            return
                "Преподношение императору XI для увеличения (или уменьшения) своего социального рейтинга\nДефолтный кулдаун 1 час, понижается наличием гигабайтов интернета";
        }
    }
}