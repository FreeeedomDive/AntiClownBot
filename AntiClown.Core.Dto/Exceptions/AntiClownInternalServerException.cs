namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownInternalServerException : AntiClownBaseException
{
    public AntiClownInternalServerException(string message) : base(message)
    {
    }

    public override int StatusCode => 500;
}