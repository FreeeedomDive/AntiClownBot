using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class WrongEventTypeException : AntiClownBadRequestException
{
    public WrongEventTypeException(string message) : base(message)
    {
    }
}