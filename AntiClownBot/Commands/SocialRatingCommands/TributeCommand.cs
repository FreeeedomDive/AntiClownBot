using System;
using System.IO;
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
            if (!Config.AreTributesOpen)
            {
                var message = isAutomatic
                    ? "Даже автоматические подношения от кошки-жены не принимаю, отъебитесь"
                    : "Я занят, отъебись";
                await e.Message.RespondAsync(message);
                return;
            }

            if (!user.IsCooldownPassed())
            {
                if (isAutomatic)
                {
                    await e.Message.RespondAsync(
                        "Ты успеть принести подношение до твой кошачья жена, подношение от кошки не учитываться");
                    return;
                }

                await e.Message.RespondAsync(
                    $"Не злоупотребляй подношение император XI {Utility.StringEmoji(":PepegaGun:")}");
                Utility.DecreaseRating(Config, user, 15, e);
                return;
            }

            var (gigabyteWorked, jadeRodWorked) = user.UpdateCooldown();

            var tributeQuality = Randomizer.GetRandomNumberBetween(
                Constants.MinTributeValue -
                user.UserItems[InventoryItem.RiceBowl] * Constants.TributeDecreaseByOneRiceBowl,
                Constants.MaxTributeValue +
                user.UserItems[InventoryItem.RiceBowl] * Constants.TributeIncreaseByOneRiceBowl);
            var response = isAutomatic ? $"Автоматическое подношение для {user.DiscordUsername}\n" : "";

            var communismChance = Utility.LogarithmicDistribution(
                Constants.LogarithmicDistributionStartValueForCommunism, user.UserItems[InventoryItem.CommunismPoster]);
            var communism = Randomizer.GetRandomNumberBetween(0, 100) < communismChance;

            var sharedUser = user;
            if (communism)
            {
                response += $"Произошел коммунизм {Utility.StringEmoji(":Pepega:")}\n";
                sharedUser = Config.Users.Values.SelectRandomItem();
                response += $"Разделение подношения с {sharedUser.DiscordUsername}";
                tributeQuality /= 2;
            }

            if (tributeQuality > 0)
            {
                response += $"Партия гордится вами!!!\n+{tributeQuality} social credit";
            }
            else if (tributeQuality < 0)
            {
                response += $"Ну и ну! Вы разочаровать партию!\n-{-tributeQuality} social credit";
            }
            else
            {
                response += "Партия не оценить ваших усилий";
            }

            if (gigabyteWorked != 0 || jadeRodWorked != 0)
            {
                response += "\nНа изменение кулдауна повлияли: ";
                response += gigabyteWorked > 0 ? $"гигабайт интернет x{gigabyteWorked} " : "";
                response += gigabyteWorked > 0 && jadeRodWorked > 0 ? " и " : "";
                response += jadeRodWorked > 0 ? $"нефритовый стержень x{gigabyteWorked}" : "";
            }

            var chance = Utility.LogarithmicDistribution(
                Constants.LogarithmicDistributionStartValueForCatWife,
                user.UserItems[InventoryItem.CatWife]);
            var isNextTributeAutomatic = Randomizer.GetRandomNumberBetween(0, 100) < chance;
            if (isNextTributeAutomatic)
                response +=
                    $"\nКошка-жена подарить тебе автоматический следующий подношение {Utility.StringEmoji(":Pog:")}";

            await e.Message.RespondAsync(response);

            if (tributeQuality > 0)
            {
                Utility.IncreaseRating(Config, user, tributeQuality, e);
                if (communism)
                {
                    Utility.IncreaseRating(Config, sharedUser, tributeQuality, e);
                }
            }
            else
            {
                Utility.DecreaseRating(Config, user, -tributeQuality, e);
                if (communism)
                {
                    Utility.DecreaseRating(Config, sharedUser, -tributeQuality, e);
                }
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