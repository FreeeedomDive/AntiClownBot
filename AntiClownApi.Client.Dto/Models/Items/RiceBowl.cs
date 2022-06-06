using AntiClownApiClient.Dto.Constants;

namespace AntiClownApiClient.Dto.Models.Items
{
    public class RiceBowl : BaseItem
    {
        public RiceBowl(Guid id) : base(id)
        {
        }

        public int NegativeRangeExtend { get; init; }
        public int PositiveRangeExtend { get; init; }

        public override string Name => StringConstants.RiceBowlName;
        public override ItemType ItemType => ItemType.Positive;

        public override Dictionary<string, string> Description() => new()
        {
            {
                "Расширение границ полученной награды с подношения",
                $"в отрицательную сторону - {NegativeRangeExtend}, " +
                $"в положительную сторону - {PositiveRangeExtend}"
            }
        };
    }
}