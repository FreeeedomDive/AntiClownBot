using AntiClownApiClient.Dto.Constants;

namespace AntiClownApiClient.Dto.Models.Items
{
    public class JadeRod : BaseItem
    {
        public JadeRod(Guid id) : base(id)
        {
        }

        public override string Name => StringConstants.JadeRodName;
        public override ItemType ItemType => ItemType.Negative;

        public int Length { get; init; }
        public int Thickness { get; init; }

        public override Dictionary<string, string> Description() => new()
        {
            {
                "Шанс увеличения кулдауна во время одной попытки", $"{NumericConstants.CooldownIncreaseChanceByOneJade}%"
            },
            {
                "Попытки увеличить кулдаун", $"{Length}"
            },
            {
                "Увеличение кулдауна в процентах", $"{Thickness}%"
            }
        };
    }
}