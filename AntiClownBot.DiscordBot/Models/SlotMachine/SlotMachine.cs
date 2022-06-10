using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils;

namespace AntiClownDiscordBotVersion2.Models.SlotMachine
{
    public class SlotMachine
    {
        public SlotMachine(
            IDiscordClientWrapper discordClientWrapper,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.randomizer = randomizer;
        }

        public async Task<SlotMachine> Create()
        {
            CircularReels = new []
            {
                new CircularReel(
                    randomizer,
                    new CellSeries(22, AllCells[0]), //x4
                    new CellSeries(13, AllCells[1]), //x15
                    new CellSeries(10, AllCells[2]), //x30
                    new CellSeries(7, AllCells[3]), //x50
                    new CellSeries(4, AllCells[4]), //x100
                    new CellSeries(2, AllCells[5]), //x500
                    new CellSeries(1, AllCells[6]), //x1500
                    new CellSeries(6, AllCells[7])), //x2-x5-x20
                new CircularReel(
                    randomizer,
                    new CellSeries(19, AllCells[0]), //x4
                    new CellSeries(16, AllCells[1]), //x15
                    new CellSeries(10, AllCells[2]), //x30
                    new CellSeries(9, AllCells[3]), //x50
                    new CellSeries(4, AllCells[4]), //x100
                    new CellSeries(2, AllCells[5]), //x500
                    new CellSeries(2, AllCells[6]), //x1500
                    new CellSeries(5, AllCells[7])), //x2-x5-x20
                new CircularReel(
                    randomizer,
                    new CellSeries(24, AllCells[0]), //x4
                    new CellSeries(14, AllCells[1]), //x15
                    new CellSeries(10, AllCells[2]), //x30
                    new CellSeries(6, AllCells[3]), //x50
                    new CellSeries(4, AllCells[4]), //x100
                    new CellSeries(2, AllCells[5]), //x500
                    new CellSeries(1, AllCells[6]), //x1500
                    new CellSeries(5, AllCells[7])) //x2-x5-x20
            };
            
            AllCells = new ISlotCell[]
            {
                new DefaultSlotCell(4, await discordClientWrapper.Emotes.FindEmoteAsync("BASED")),
                new DefaultSlotCell(15  , await discordClientWrapper.Emotes.FindEmoteAsync("popCat")),
                new DefaultSlotCell(25  , await discordClientWrapper.Emotes.FindEmoteAsync("peepoClap")),
                new DefaultSlotCell(50  , await discordClientWrapper.Emotes.FindEmoteAsync("ricardoFlick")),
                new DefaultSlotCell(100 , await discordClientWrapper.Emotes.FindEmoteAsync("BOOBA")),
                new DefaultSlotCell(500 , await discordClientWrapper.Emotes.FindEmoteAsync("RainbowPls")),
                new DefaultSlotCell(1500, await discordClientWrapper.Emotes.FindEmoteAsync("PATREGO")),
                new CherrySlotCell(2, 5, 20, await discordClientWrapper.Emotes.FindEmoteAsync("")),
            };

            return this;
        }

        public ISlotCell[] AllCells { get; private set; }

        public CircularReel[] CircularReels { get; private set; }

        public string Description() => "Таблица выплат:\n" + string.Join("\n", AllCells.Select(c => c.Description()));

        public SlotMachineResult Play(int bet)
        {
            var cells = GetRandomCells().ToList();
            var win = CalcWin(cells.Select(c => c[1]), bet);
            var result = new SlotMachineResult(cells, win);
            return result;
        }

        private int CalcWin(IEnumerable<ISlotCell> cells, int bet)
        {
            var cellsList = cells.ToList();
            var cherries = cellsList.OfType<CherrySlotCell>().ToList();
            if (cherries.Any())
            {
                return cherries.Count switch
                {
                    1 => bet * cherries[0].X1Multiplier,
                    2 => bet * cherries[0].X2Multiplier,
                    3 => bet * cherries[0].X3Multiplier,
                    _ => throw new ArgumentOutOfRangeException("пиздец чел сдохни")
                };
            }

            if (cellsList.Count(c => c.Equals(cellsList[0])) == 3 && cellsList[0] is DefaultSlotCell cell)
            {
                return cell.Multiplier * bet;
            }

            return 0;
        }

        private IEnumerable<List<ISlotCell>> GetRandomCells()
        {
            return CircularReels
                .Select(reel => new { reel, winPos = randomizer.GetRandomNumberBetween(0, reel.CellCount) })
                .Select(@t => new List<ISlotCell>
                {
                    @t.reel.GetCell(@t.winPos - 1),
                    @t.reel.GetCell(@t.winPos),
                    @t.reel.GetCell(@t.winPos + 1)
                })
                .ToList();
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IRandomizer randomizer;
    }
}