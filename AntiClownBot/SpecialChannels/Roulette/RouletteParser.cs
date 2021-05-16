using AntiClownBot.SpecialChannels.Roulette.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.SpecialChannels.Roulette
{
    public class RouletteParser : SpecialChannelParser
    {
        public RouletteParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new RouletteBet(client, configuration),
                new RoulettePlay(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        public override async void Parse(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                return;
            }
            var message = command.Execute(e, user);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        public override string Help(MessageCreateEventArgs e)
        {
            return "bet [размер ставки][single | red | black | odd | even][номер сектора если ставка single]\n" +
                "play - Команда чтобы запустить рулетку и раздать очки";
        }
    }
}
