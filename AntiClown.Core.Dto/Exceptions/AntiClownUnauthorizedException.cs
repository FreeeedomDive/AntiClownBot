namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownUnauthorizedException : AntiClownBaseException
{
    public AntiClownUnauthorizedException(string message) : base(message)
    {
    }

    public override int StatusCode => 401;
}