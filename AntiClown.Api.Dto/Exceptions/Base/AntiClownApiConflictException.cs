namespace AntiClown.Api.Dto.Exceptions.Base;

public class AntiClownApiConflictException : AntiClownApiBaseException
{
    public AntiClownApiConflictException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 409;
}