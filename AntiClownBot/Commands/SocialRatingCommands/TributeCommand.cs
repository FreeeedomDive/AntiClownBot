using System;
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

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!user.IsCooldownPassed())
            {
                await e.Message.RespondAsync(
                    $"Не злоупотребляй подношение император XI {DiscordEmoji.FromName(DiscordClient, ":PepegaGun:")}");
                Utility.DecreaseRating(Config, user, 15, e);
                return;
            }

            user.UpdateCooldown();
            var tributeQuality = Randomizer.GetRandomNumberBetween(-40, 100);
            if (tributeQuality > 0)
            {
                await e.Message.RespondAsync($"Партия гордится тобой!!!\n+{tributeQuality} social credit");
                Utility.IncreaseRating(Config, user, tributeQuality, e);
            }
            else if (tributeQuality < 0)
            {
                await e.Message.RespondAsync(
                    $"Ну и ну! Вы разочаровать партию!\n-{-tributeQuality} social credit");
                Utility.DecreaseRating(Config, user, -tributeQuality, e);
            }
            else
            {
                await e.Message.RespondAsync("Партия не оценить ваших усилий");
            }
        }

        public override string Help()
        {
            return
                "Преподношение императору XI для увеличения (или уменьшения) своего социального рейтинга\nКулдаун 1 час";
        }
    }
}