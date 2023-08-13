namespace AntiClown.Core.Dto.Exceptions;

public abstract class AntiClownBadRequestException : AntiClownBaseException
{
    public AntiClownBadRequestException(string message) : base(message)
    {
    }

    public override int StatusCode => 400;
}