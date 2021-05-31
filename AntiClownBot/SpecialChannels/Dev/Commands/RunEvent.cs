using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBot.Events;
using AntiClownBot.Events.CloseTributeEvent;
using AntiClownBot.Events.CloseTributeEvent.RelatedOpenTributesEvent;
using AntiClownBot.Events.DailyEvents;
using AntiClownBot.Events.GuessNumberEvent;
using AntiClownBot.Events.LotteryEvent;
using AntiClownBot.Events.MaliMaliEvent;
using AntiClownBot.Events.RemoveCooldownEvent;
using AntiClownBot.Events.ShopEvent;
using AntiClownBot.Events.TransfusionEvent;
using DSharpPlus;
using EventHandler = AntiClownBot.Events.EventHandler;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class RunEvent : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        private List<BaseEvent> _events = new List<BaseEvent>
        {
            new CloseTributesEvent(),
            new DailyStatisticsEvent(),
            new GuessNumberEvent(),
            new LotteryEvent(),
            new MaliMaliEvent(),
            new OpenTributesEvent(),
            new PayoutsDailyEvent(),
            new RemoveCooldownEvent(),
            new ShopEvent(),
            new TransfusionEvent()
        };
        public RunEvent(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "runevent";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var eventName = e.Message.Content.Split(' ').Skip(1).First();
            BaseEvent eventToHandle;
            switch (eventName)
            {
                case "closetributes":
                    eventToHandle = _events[0];
                    break;
                case "dailystats":
                    eventToHandle = _events[1];
                    break;
                case "guessnumber":
                    eventToHandle = _events[2];
                    break;
                case "lottery":
                    eventToHandle = _events[3];
                    break;
                case "malimali":
                    eventToHandle = _events[4];
                    break;
                case "opentributes":
                    eventToHandle = _events[5];
                    break;
                case "payouts":
                    eventToHandle = _events[6];
                    break;
                case "removecooldown":
                    eventToHandle = _events[7];
                    break;
                case "shop":
                    eventToHandle = _events[8];
                    break;
                case "transfusion":
                    eventToHandle = _events[9];
                    break;
                default:
                    eventToHandle = null;
                    break;
            }

            if (eventToHandle == null)
            {
                return "Такого ивента нет";
            }

            if (Config.EventPossibleTimes[eventName] > DateTime.Now)
            {
                return "Кулдаун для этого ивента не прошёл";
            }
            var tempTime = DateTime.Now.AddMilliseconds(eventToHandle.EventCooldown);
            if (tempTime > EventHandler.NextEventPossibleTime)
            {
                EventHandler.NextEventPossibleTime = tempTime;
            }

            var thread = new Thread(_ =>
            {
                eventToHandle.ExecuteAsync();
            });
            thread.IsBackground = true;
            thread.Start();
            return "Ивент был запущен успешно";
        }
    }
}
