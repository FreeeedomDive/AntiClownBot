using System;
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
            if (!user.IsCooldownPassed() && !isAutomatic)
            {
                await e.Message.RespondAsync(
                    $"Не злоупотребляй подношение император XI {Utility.StringEmoji(":PepegaGun:")}");
                Utility.DecreaseRating(Config, user, 15, e);
                return;
            }

            user.UpdateCooldown();
            const int tributeDecreaseByOneRiceBowl = 2;
            const int tributeIncreaseByOneRiceBowl = 5;
            var tributeQuality = Randomizer.GetRandomNumberBetween(
                -40 - user.UserItems[InventoryItem.RiceBowl] * tributeDecreaseByOneRiceBowl, 
                100 + user.UserItems[InventoryItem.RiceBowl] * tributeIncreaseByOneRiceBowl);
            var response = isAutomatic ? $"Автоматическое подношение для {user.DiscordUsername}\n" : "";
            if (tributeQuality > 0)
            {
                response += $"Партия гордится тобой!!!\n+{tributeQuality} social credit";
            }
            else if (tributeQuality < 0)
            {
                response += $"Ну и ну! Вы разочаровать партию!\n-{-tributeQuality} social credit";
            }
            else
            {
                response += "Партия не оценить ваших усилий";
            }

            var isNextTributeAutomatic =
                Randomizer.GetRandomNumberBetween(0, 100) < Utility.LogarithmicDistribution(16, user.UserItems[InventoryItem.CatWife]);
            if (isNextTributeAutomatic)
                response +=
                    $"\nКошка-жена подарить тебе автоматический следующий подношение {Utility.StringEmoji(":Pog:")}";

            await e.Message.RespondAsync(response);
            
            if (tributeQuality > 0)
                Utility.IncreaseRating(Config, user, tributeQuality, e);
            else
                Utility.DecreaseRating(Config, user, -tributeQuality, e);

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