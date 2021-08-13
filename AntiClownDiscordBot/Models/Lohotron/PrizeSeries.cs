using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lohotron
{
    public class PrizeSeries
    {
        public int Count { get; }
        public ILohotronPrize Prize { get; }
        public PrizeSeries(int count, ILohotronPrize prize)
        {
            Count = count;
            Prize = prize;
        }
    }
}
