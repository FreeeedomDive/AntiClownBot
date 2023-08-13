using AntiClown.Api.Dto.Economies;

namespace AntiClown.Messages.Dto.Tributes;

public class TributeMessageDto
{
    public Guid UserId { get; set; }
    public TributeDto Tribute { get; set; }
}