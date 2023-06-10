namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownConflictException : AntiClownBaseException
{
    public AntiClownConflictException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 409;
}