using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AntiClownBot.Events;
using AntiClownBot.Events.DailyEvents;
using AntiClownBot.Events.GuessNumberEvent;
using AntiClownBot.Events.LotteryEvent;
using AntiClownBot.Events.MaliMaliEvent;
using AntiClownBot.Events.RaceEvent;
using AntiClownBot.Events.RemoveCooldownEvent;
using AntiClownBot.Events.TransfusionEvent;
using DSharpPlus;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class RunEvent : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        private List<BaseEvent> _events = new List<BaseEvent>
        {
            new DailyStatisticsEvent(),
            new GuessNumberEvent(),
            new LotteryEvent(),
            new MaliMaliEvent(),
            new PayoutsDailyEvent(),
            new RemoveCooldownEvent(),
            new TransfusionEvent(),
            new RaceEvent()
        };
        public RunEvent(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "runevent";

        public string Execute(MessageCreateEventArgs e)
        {
            var eventName = e.Message.Content.Split(' ').Skip(1).First();
            BaseEvent eventToHandle = eventName switch
            {
                "dailystats" => _events.OfType<DailyStatisticsEvent>().First(),
                "guessnumber" => _events.OfType<GuessNumberEvent>().First(),
                "lottery" => _events.OfType<LotteryEvent>().First(),
                "malimali" => _events.OfType<MaliMaliEvent>().First(),
                "payouts" => _events.OfType<PayoutsDailyEvent>().First(),
                "removecooldown" => _events.OfType<RemoveCooldownEvent>().First(),
                "transfusion" => _events.OfType<TransfusionEvent>().First(),
                "race" => _events.OfType<RaceEvent>().First(),
                _ => null
            };

            if (eventToHandle == null)
            {
                return "Такого ивента нет";
            }

            var thread = new Thread(_ =>
            {
                eventToHandle.ExecuteAsync();
            })
            {
                IsBackground = true
            };
            thread.Start();
            return "Ивент был запущен успешно";
        }
    }
}
