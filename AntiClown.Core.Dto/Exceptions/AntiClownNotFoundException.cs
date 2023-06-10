namespace AntiClown.Core.Dto.Exceptions;

public class AntiClownNotFoundException : AntiClownBaseException
{
    public AntiClownNotFoundException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 404;
}