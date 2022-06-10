using System.Collections.Generic;

namespace AntiClownBot.Models.Lohotron
{
    public class Lohotron
    {
        public readonly List<ulong> UsersId = new();

        private static readonly ILohotronPrize[] AllPrizes =
        {
            new DefaultLohotronPrize(),
            new CreditsLohotronPrize(1),
            new CreditsLohotronPrize(100),
            new CreditsLohotronPrize(200),
            new CreditsLohotronPrize(250),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(-500),
            new LootBoxLohotronPrize()
        };

        private static readonly Wheel Wheel =
            new(
                new(40, AllPrizes[0]),   // Nothing
                new(30, AllPrizes[1]),              // +1
                new(14, AllPrizes[2]),              // +100
                new(7, AllPrizes[3]),               // +200
                new(1, AllPrizes[4]),               // +250
                new(1, AllPrizes[5]),               // +500
                new(1, AllPrizes[6]),               // +500
                new(1, AllPrizes[7]),               // +500
                new(3, AllPrizes[8]),               // -500
                new (6, AllPrizes[9])               // Lootbox
            );

        public ILohotronPrize Play()
        {
            return Wheel.GetPrize(Randomizer.GetRandomNumberBetween(0, Wheel.PrizesCount));
        }
    }
}