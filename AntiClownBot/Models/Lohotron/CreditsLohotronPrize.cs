using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lohotron
{
    public class CreditsLohotronPrize : ILohotronPrize
    {
        public string Name { get => "Credits"; }
        public int Count;
        public CreditsLohotronPrize(int count)
        {
            Count = count;
        }
    }
}
