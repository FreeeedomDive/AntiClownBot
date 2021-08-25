using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;
using System.Text;
using AntiClownBot.Helpers;

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

        public string Execute(MessageCreateEventArgs e)
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
            if (Configuration.GetUserBalance(e.Author.Id) - bet < -1000)
            {
                return "Долг больше 1к";
            }

            var member = Configuration.GetServerMember(e.Author.Id);
            
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{member.ServerOrUsername()}:\n");
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
                Config.ChangeBalance(e.Author.Id, bet, "Победа в ролле");
                stringBuilder.Append($"\n{member.ServerOrUsername()} выиграл {bet}");
            }
            else
            {
                Config.ChangeBalance(e.Author.Id, -bet, "Проигрыш в ролле");
                stringBuilder.Append($"\n{member.ServerOrUsername()} проиграл {bet}");
            }
            return stringBuilder.ToString();
        }
    }
}
