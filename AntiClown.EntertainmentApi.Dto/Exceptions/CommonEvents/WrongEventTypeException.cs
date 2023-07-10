using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;

public class WrongEventTypeException : AntiClownBadRequestException
{
    public WrongEventTypeException(string message) : base(message)
    {
    }
}