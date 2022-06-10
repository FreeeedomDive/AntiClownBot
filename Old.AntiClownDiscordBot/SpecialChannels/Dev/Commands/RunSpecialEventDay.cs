using System;
using System.Linq;
using AntiClownBot.Events.SpecialEventDays;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class RunSpecialEventDay : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        
        public RunSpecialEventDay(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        
        public string Name => "rundayevents";
        public string Execute(MessageCreateEventArgs e)
        {
            return "хуй";
            
            /*var eventName = e.Message.Content.Split(' ').Skip(1).First();
            var eventDayType = eventName switch
            {
                "lottery" => EventDayType.LotteryDay,
                "race" => EventDayType.RaceDay,
                "common" => EventDayType.CommonDay,
                _ => throw new ArgumentOutOfRangeException(nameof(eventName))
            };
            
            Config.EventHandler.StartSpecialEventDay(eventDayType);
            return "done";*/
        }
    }
}