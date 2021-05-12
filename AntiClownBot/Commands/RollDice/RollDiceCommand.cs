using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Text;

namespace AntiClownBot.Commands.RollDice
{
    public class RollDiceCommand : BaseCommand
    {
        public RollDiceCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var args = e.Message.Content.Split(' ').Skip(1).ToList();
            if(args.Count != 1)
            {
                await e.Message.RespondAsync("Нормально пиши. чел");
                return;
            }
            var betParsed = int.TryParse(args.First(), out var bet);
            if(!betParsed || bet <= 0)
            {
                await e.Message.RespondAsync("Нормально пиши. чел");
                return;
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{user.DiscordUsername}:\n");
            var playerResult = 0;
            var imperatorResult = 0;
            for(var i = 0; i < 3; i++)
            {
                var dice = Randomizer.GetRandomNumberBetween(1, 7);
                playerResult += dice;
                stringBuilder.Append($"{dice} ");
            }
            stringBuilder.Append($"\nИмператорXI:\n");
            for (var i = 0; i < 3; i++)
            {
                var dice = Randomizer.GetRandomNumberBetween(1, 7);
                imperatorResult += dice;
                stringBuilder.Append($"{dice} ");
            }
            if(playerResult > imperatorResult)
            {
                user.ChangeRating(bet);
                stringBuilder.Append($"\n{user.DiscordUsername} выиграл {bet} и теперь имеет {user.SocialRating} кредитов.");
            }
            else
            {
                user.ChangeRating(-bet);
                stringBuilder.Append($"\n{user.DiscordUsername} проиграл {bet} и теперь имеет {user.SocialRating} кредитов.");
            }
            await e.Message.RespondAsync(stringBuilder.ToString());
        }

        public override string Help()
        {
            return "!rolldice {ставка}. Бросить 3 кубика, если результат больше чем у бота - выигрываешь, если меньше или равно - проигрываешь.";
        }
    }
}
