﻿using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBot.SpecialChannels.RollDice.Commands;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.RollDice
{
    public class RollDiceParser : SpecialChannelParser
    {
        public RollDiceParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new Roll(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        
        public override async void Parse(MessageCreateEventArgs e)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                return;
            }
            var message = command.Execute(e);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        
        public override string Help(MessageCreateEventArgs e)
        {
            return "roll {ставка}";
        }
    }
}