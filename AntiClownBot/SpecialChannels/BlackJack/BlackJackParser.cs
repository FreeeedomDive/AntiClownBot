using AntiClownBot.SpecialChannels.BlackJack.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.SpecialChannels.BlackJack
{
    public class BlackJackParser : SpecialChannelParser
    {
        public BlackJackParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new BlackJackJoin(client, configuration),
                new BlackJackLeave(client, configuration),
                new BlackJackStart(client, configuration),
                new BlackJackHit(client, configuration),
                new BlackJackDouble(client, configuration),
                new BlackJackStand(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        public override async void Parse(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                await e.Message.RespondAsync("Чел, такой команды нет");
                return;
            }
            var message = command.Execute(e, user);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        public override string Help()
        {
            return "";
        }
    }
}
