using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Events.DailyEvents
{
    public class DailyStatisticsEvent : BaseEvent
    {
        public override int EventCooldown => 10 * 1000;
        public override async void ExecuteAsync()
        {
            var text = BackStory();
            await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            Config.CheckCurrentDay();
            Config.Save();
        }

        protected override string BackStory()
        {
            return "Добрый вечер, Clown-City!\n" +
                $"Вчерашний посчёт пидоров закончился на {Config.DailyStatistics.PidorCollected}\n" +
                $"Спонсор каждого из них - Я!\n" +
                $"Граждане наш Clown-City за день заработать {Config.DailyStatistics.CreditsCollected}\n" +
                $"Мною было проводить {Config.DailyStatistics.EventsCount} событий\n" +
                $"Мажор дня - {Utility.KeyValuePairToString(Config.DailyStatistics.CreditsById.OrderBy(x => x.Value).Last())}\n" +
                $"Бомж дня - {Utility.KeyValuePairToString(Config.DailyStatistics.CreditsById.OrderBy(x => x.Value).First())}";
        }
    }
}
