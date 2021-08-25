using System.Collections.Generic;

namespace AntiClownBot.Models.Lohotron
{
    public class Lohotron
    {
        public readonly List<ulong> UsersId = new();

        private static readonly ILohotronPrize[] AllPrizes =
        {
            new DefaultLohotronPrize(),
            new CreditsLohotronPrize(100),
            new CreditsLohotronPrize(200),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(-500),
            new CreditsLohotronPrize(-500),
            new CreditsLohotronPrize(1),
        };

        private static readonly Wheel Wheel =
            new(
                new(40, AllPrizes[0]), //Nothing
                new(30, AllPrizes[1]), //+100 Credits
                new(14, AllPrizes[2]), //+200 Credits
                new(1, AllPrizes[3]), //CatWife
                new(1, AllPrizes[4]), //DogWife
                new(1, AllPrizes[5]), //Gigabyte
                new(1, AllPrizes[6]), //RiceBowl
                new(3, AllPrizes[7]), //CommunismPoster
                new(3, AllPrizes[8]), //JadeRod
                new (6, AllPrizes[9]) //Lootbox
            );

        public ILohotronPrize Play()
        {
            return Wheel.GetPrize(Randomizer.GetRandomNumberBetween(0, Wheel.PrizesCount));
        }
    }
}