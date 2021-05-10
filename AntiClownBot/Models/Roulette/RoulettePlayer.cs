namespace Roulette
{
    public struct RoulettePlayer
    {
        public ulong Id { get; private set; }

        public RoulettePlayer(ulong id)
        {
            Id = id;
        }
    }
}