using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace AntiClownBot.Events.MaliMaliEvent
{
    public class MaliMaliEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            var channels = DiscordClient.Guilds[277096298761551872].Channels.Values
                .Where(ch => ch.Type == ChannelType.Voice && ch.Users.ToList().Count > 0).ToList();
            var channel = channels.SelectRandomItem();
            var vnext = Utility.Voice;
            var vnc = vnext.GetConnection(DiscordClient.Guilds[277096298761551872]);
            if (vnc != null)
            {
                return;
            }

            vnc = await vnext.ConnectAsync(channel);
            Task.Delay(1 * 60 * 1000);
            vnc.Disconnect();
        }

        protected override string BackStory()
        {
            throw new NotImplementedException();
        }
    }
}
