using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Helpers;

namespace AntiClownBot.Events.DailyEvents
{
    public class DailyStatisticsEvent : BaseEvent
    {
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
            var (majorKey, majorValue) = Config.DailyStatistics.CreditsById.OrderBy(x => x.Value).Last();
            var majorUserName = Configuration.GetServerMember(majorKey).ServerOrUsername();
            var (bomjKey, bomjValue) = Config.DailyStatistics.CreditsById.OrderBy(x => x.Value).First();
            var bomjUserName = Configuration.GetServerMember(bomjKey).ServerOrUsername();
            
            return "Добрый вечер, Clown-City!\n" +
                $"Граждане наш Clown-City за день заработать {Config.DailyStatistics.CreditsCollected}\n" +
                $"Мною было проводить {Config.DailyStatistics.EventsCount} событий\n" +
                $"Мажор дня - {majorUserName} : {majorValue}\n" +
                $"Бомж дня - {bomjUserName} : {bomjValue}";
        }
    }
}
