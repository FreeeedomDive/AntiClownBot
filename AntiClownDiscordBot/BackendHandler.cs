using System.Threading.Tasks;
using AntiClownBot.Models;
using ApiWrapper.Wrappers;
using DSharpPlus.Entities;

namespace AntiClownBot
{
    public class BackendHandler
    {
        private const int Period = 1 * 60 * 1000;
        public static bool IsHandlerWorking { get; set; } = true;

        public static async void BackendMessagesLongPolling()
        {
            while (IsHandlerWorking)
            {
                await Task.Delay(Period);
                var isServerWorking = GlobalStateApi.IsBackendWorking();
                if (!isServerWorking)
                {
                    var admin = await Utility.Client.Guilds[Constants.GuildId].GetMemberAsync(259306088040628224);
                    var messageBuilder = new DiscordMessageBuilder
                    {
                        Content = $"{admin.Mention} кажется, сервер прилёг {Utility.Emoji(":monkaX:")}"
                    };
                    messageBuilder.WithAllowedMention(UserMention.All);
                    await Utility.Client.Guilds[Constants.GuildId].GetChannel(Constants.BotChannelId)
                        .SendMessageAsync(messageBuilder);
                    continue;
                }

                var stalledAutoTributes = GlobalStateApi.GetAutomaticTributes();
                if (stalledAutoTributes == null || stalledAutoTributes.Count == 0)
                    continue;
                stalledAutoTributes.ForEach(async tribute =>
                {
                    await Utility.Client
                        .Guilds[Constants.GuildId]
                        .GetChannel(Constants.TributeChannelId)
                        .SendMessageAsync(Tribute.MakeEmbedForTribute(tribute));
                });
            }
        }
    }
}