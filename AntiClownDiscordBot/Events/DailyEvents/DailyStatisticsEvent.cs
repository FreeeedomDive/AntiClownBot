using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var majorUserName = DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(majorKey).Result;
            var (bomjKey, bomjValue) = Config.DailyStatistics.CreditsById.OrderBy(x => x.Value).First();
            var bomjUserName = DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(bomjKey).Result;
            
            return "Добрый вечер, Clown-City!\n" +
                $"Вчерашний посчёт пидоров закончился на {Config.DailyStatistics.PidorCollected}\n" +
                $"Спонсор каждого из них - Я!\n" +
                $"Граждане наш Clown-City за день заработать {Config.DailyStatistics.CreditsCollected}\n" +
                $"Мною было проводить {Config.DailyStatistics.EventsCount} событий\n" +
                $"Мажор дня - {majorUserName} : {majorValue}\n" +
                $"Бомж дня - {bomjUserName} : {bomjValue}";
        }
    }
}
