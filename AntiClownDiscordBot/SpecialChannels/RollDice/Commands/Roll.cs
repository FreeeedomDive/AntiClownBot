using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.RollDice.Commands
{
    public class Roll : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public Roll(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "roll";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var args = e.Message.Content.Split(' ').Skip(1).ToList();
            if (args.Count != 1)
            {
                return "Нормально пиши. чел";
            }
            var betParsed = int.TryParse(args.First(), out var bet);
            if (!betParsed || bet <= 0)
            {
                return "Нормально пиши. чел";
            }
            if (user.SocialRating - bet < -1000)
            {
                return "Долг больше 1к";
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{user.DiscordUsername}:\n");
            var playerResult = 0;
            var imperatorResult = 0;
            for (var i = 0; i < 2; i++)
            {
                var dice = Randomizer.GetRandomNumberBetween(1, 7);
                playerResult += dice;
                stringBuilder.Append($"{dice} ");
            }
            stringBuilder.Append($"\nИмператорXI:\n");
            for (var i = 0; i < 2; i++)
            {
                var dice = Randomizer.GetRandomNumberBetween(1, 7);
                imperatorResult += dice;
                stringBuilder.Append($"{dice} ");
            }
            if (playerResult > imperatorResult)
            {
                user.ChangeRating(bet);
                stringBuilder.Append($"\n{user.DiscordUsername} выиграл {bet} и теперь имеет {user.SocialRating} кредитов.");
            }
            else
            {
                user.ChangeRating(-bet);
                stringBuilder.Append($"\n{user.DiscordUsername} проиграл {bet} и теперь имеет {user.SocialRating} кредитов.");
            }
            return stringBuilder.ToString();
        }
    }
}
