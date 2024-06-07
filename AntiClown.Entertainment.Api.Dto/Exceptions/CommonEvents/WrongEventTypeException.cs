using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class WrongEventTypeException : BadRequestException
{
    public WrongEventTypeException(string message) : base(message)
    {
    }
}