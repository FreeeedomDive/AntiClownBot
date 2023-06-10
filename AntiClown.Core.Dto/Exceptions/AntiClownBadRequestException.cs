namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownBadRequestException : AntiClownBaseException
{
    public AntiClownBadRequestException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 400;
}