using AntiClownApiClient.Dto.Constants;

namespace AntiClownApiClient.Dto.Models.Items
{
    public class DogWife: BaseItem
    {
        public DogWife(Guid id) : base(id)
        {
        }
        
        public override string Name => StringConstants.DogWifeName;
        public override ItemType ItemType => ItemType.Positive;

        public int LootBoxFindChance { get; init; }
        
        public override Dictionary<string, string> Description() => new()
        {
            {"Шанс получить лутбокс во время подношения", $"{(double)LootBoxFindChance / 10}%"}
        };
    }
}