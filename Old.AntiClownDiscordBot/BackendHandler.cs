﻿using System.Threading.Tasks;
using AntiClownBot.Helpers;
using AntiClownBot.Models;
using ApiWrapper.Wrappers;
using DSharpPlus.Entities;

namespace AntiClownBot
{
    public static class BackendHandler
    {
        private const int Period = 1 * 30 * 1000;
        public static bool IsHandlerWorking { get; set; } = true;

        public static async void BackendMessagesLongPolling()
        {
            while (IsHandlerWorking)
            {
                await Task.Delay(Period);
                var isServerWorking = GlobalStateApi.IsBackendWorking();
                if (!isServerWorking)
                {
                    var admin = Configuration.GetServerMember(259306088040628224);
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
                    if (Tribute.TryMakeEmbedForTribute(tribute, out var tributeEmbed))
                        await Utility.Client
                            .Guilds[Constants.GuildId]
                            .GetChannel(Constants.TributeChannelId)
                            .SendMessageAsync(tributeEmbed);
                });
            }
        }
    }
}