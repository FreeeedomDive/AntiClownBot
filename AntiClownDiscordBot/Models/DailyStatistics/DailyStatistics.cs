using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.DailyStatistics
{
    public class DailyStatistics
    {
        public int CreditsCollected;
        public int PidorCollected;
        public int EventsCount;
        public Dictionary<ulong, int> CreditsById;

        public DailyStatistics()
        {
            CreditsCollected = 0;
            PidorCollected = 0;
            EventsCount = 0;
            CreditsById = new Dictionary<ulong, int>();
        }
        public void ChangeUserCredits(ulong id, int count)
        {
            if(!CreditsById.ContainsKey(id))
            {
                CreditsById.Add(id, count);
            }
            else
            {
                CreditsById[id] += count;
            }
        }
    }
}
