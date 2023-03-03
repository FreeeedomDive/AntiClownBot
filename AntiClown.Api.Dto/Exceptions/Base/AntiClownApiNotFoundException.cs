namespace AntiClown.Api.Dto.Exceptions.Base;

public class AntiClownApiNotFoundException : AntiClownApiBaseException
{
    public AntiClownApiNotFoundException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 404;
}