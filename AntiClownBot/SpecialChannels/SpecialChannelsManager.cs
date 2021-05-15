using AntiClownBot.SpecialChannels.BlackJack;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;

namespace AntiClownBot.SpecialChannels
{
    public class SpecialChannelsManager
    {
        private readonly Dictionary<ulong, SpecialChannelParser> _specialChannels;

        public SpecialChannelsManager(DiscordClient client, Configuration configuration)
        {
            _specialChannels = new Dictionary<ulong, SpecialChannelParser>
            {
                {843065708023382036, new BlackJackParser(client, configuration) }
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

        public async void ParseMessage(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!GetChannelByName(e.Channel.Id, out var parser))
            {
                await e.Message.RespondAsync($"Я хз что происходит, но {e.Channel.Id} канала не существует");
                return;
            }
            parser.Parse(e, user);
        }
    }
}
