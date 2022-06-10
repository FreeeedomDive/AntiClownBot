namespace AntiClownDiscordBotVersion2.Models.Roulette
{
    public struct RoulettePlayer
    {
        public ulong Id { get; private set; }

        public RoulettePlayer(ulong id)
        {
            Id = id;
        }

        public bool Equals(RoulettePlayer p2)
        {
            return Id == p2.Id;
        }
    }
}