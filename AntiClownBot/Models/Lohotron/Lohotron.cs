using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lohotron
{
    public class Lohotron
    {
        public List<ulong> UsersId = new();

        private static readonly ILohotronPrize[] AllPrizes =
        {
            new DefaultLohotronPrize(),
            new CreditsLohotronPrize(-100),
            new CreditsLohotronPrize(-200),
            new CreditsLohotronPrize(100),
            new CreditsLohotronPrize(200),
            new ItemLohotronPrize(InventoryItem.CatWife),
            new ItemLohotronPrize(InventoryItem.DogWife),
            new ItemLohotronPrize(InventoryItem.Gigabyte),
            new ItemLohotronPrize(InventoryItem.RiceBowl),
            new ItemLohotronPrize(InventoryItem.CommunismPoster),
            new ItemLohotronPrize(InventoryItem.JadeRod)
        };

        private static readonly Wheel Wheel =
            new(
                new(32, AllPrizes[0]), //Nothing
                new(16, AllPrizes[1]), //-100 Credits
                new(12, AllPrizes[2]), //-200 Credits
                new(16, AllPrizes[3]), //+100 Credits
                new(12, AllPrizes[4]), //+200 Credits
                new(1, AllPrizes[5]), //CatWife
                new(1, AllPrizes[6]), //DogWife
                new(1, AllPrizes[7]), //Gigabyte
                new(1, AllPrizes[8]), //RiceBowl
                new(4, AllPrizes[9]), //CommunismPoster
                new(4, AllPrizes[10]) //JadeRod
            );

        public ILohotronPrize Play()
        {
            return Wheel.GetPrize(Randomizer.GetRandomNumberBetween(0, Wheel.PrizesCount));
        }
    }
}