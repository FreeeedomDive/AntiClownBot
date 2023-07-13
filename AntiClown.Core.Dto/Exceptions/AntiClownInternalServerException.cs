namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownInternalServerException : AntiClownBaseException
{
    public AntiClownInternalServerException(string message, Exception baseException) : base(message, baseException)
    {
    }

    public override int StatusCode => 500;
}