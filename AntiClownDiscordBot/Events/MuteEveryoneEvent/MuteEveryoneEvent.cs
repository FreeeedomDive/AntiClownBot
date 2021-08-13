using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Net.Models;

namespace AntiClownBot.Events.MuteEveryoneEvent
{
    public class MuteEveryoneEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            var text = BackStory();
            await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            var voiceUsers = DiscordClient.Guilds[277096298761551872].VoiceStates
                .Where(kvp => kvp.Value.Channel != null).ToList();
            if (!voiceUsers.Any())
            {
                return;
            }
            var channel = voiceUsers.First().Value.Channel;
            foreach (var user in voiceUsers)
            {
                if (user.Value.Channel == channel)
                {
                    var member = DiscordClient
                        .Guilds[277096298761551872].GetMemberAsync(user.Key);
                    await member.Result.ModifyAsync(model => { model.Muted = true;
                        model.VoiceChannel = DiscordClient.Guilds[277096298761551872]
                            .GetChannel(689120451984621605);
                    });
                }
            }
        }

        protected override string BackStory()
        {
            return $"{Utility.Emoji(":pepeLaugh:")}";
        }
    }
}
