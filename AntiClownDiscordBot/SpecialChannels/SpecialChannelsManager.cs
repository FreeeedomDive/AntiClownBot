﻿using AntiClownBot.SpecialChannels.BlackJack;
using AntiClownBot.SpecialChannels.Gambling;
using AntiClownBot.SpecialChannels.RollDice;
using AntiClownBot.SpecialChannels.Roulette;
using AntiClownBot.SpecialChannels.Slot;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using AntiClownBot.Helpers;
using AntiClownBot.SpecialChannels.Dev;

namespace AntiClownBot.SpecialChannels
{
    public class SpecialChannelsManager
    {
        private readonly Dictionary<ulong, SpecialChannelParser> _specialChannels;

        public SpecialChannelsManager(DiscordClient client, Configuration configuration)
        {
            _specialChannels = new Dictionary<ulong, SpecialChannelParser>
            {
                {843065708023382036, new BlackJackParser(client, configuration) },
                {843051370168320002, new GamblingParser(client, configuration) },
                {843051438594064384, new RollDiceParser(client, configuration) },
                {843051483892023316, new RouletteParser(client, configuration) },
                {843051525931532298, new SlotParser(client,configuration) },
                {848100451949608970, new DevParser(client, configuration)}
            };
        }
        public bool GetChannelByName(ulong name, out SpecialChannelParser parser)
        {
            if (_specialChannels.ContainsKey(name))
            {
                parser = _specialChannels[name];
                return true;
            }

            parser = null;
            return false;
        }

        public IEnumerable<ulong> AllChannels => _specialChannels.Keys;

        public async void ParseMessage(MessageCreateEventArgs e)
        {
            if (Configuration.IsMaintenanceMode && e.Author.Id != 259306088040628224)
            {
                await e.Message.RespondAsync($"Пока не отвечаю {Utility.Emoji(":NOPERS:")}");
                return;
            }
            if (!GetChannelByName(e.Channel.Id, out var parser))
            {
                await e.Message.RespondAsync($"Я хз что происходит, но {e.Channel.Id} канала не существует");
                return;
            }
            if (e.Message.Content.StartsWith("help"))
            {
                await e.Message.RespondAsync(parser.Help(e));
                return;
            }
            parser.Parse(e);
        }
    }
}