namespace AntiClownDiscordBotVersion2.Models.Lohotron
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
