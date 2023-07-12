namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownInternalServerError : AntiClownBaseException
{
    public AntiClownInternalServerError(string message, Exception baseException) : base(message, baseException)
    {
    }

    public override int StatusCode => 500;
}