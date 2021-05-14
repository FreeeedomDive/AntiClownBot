using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lohotron
{
    public class Wheel
    {
        public List<ILohotronPrize> Prizes { get; } = new ();
        public int PrizesCount => Prizes.Count;

        public Wheel(params PrizeSeries[] prizes)
        {
            foreach(var prize in prizes)
            {
                for (var i = 0; i < prize.Count; i++)
                {
                    Prizes.Add(prize.Prize);

                }
            }
        }
        public ILohotronPrize GetPrize(int pos)
        {
            return Prizes[pos];
        }
    }
}
