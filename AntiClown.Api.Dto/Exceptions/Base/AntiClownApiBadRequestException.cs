namespace AntiClown.Api.Dto.Exceptions.Base;

public class AntiClownApiBadRequestException : AntiClownApiBaseException
{
    public AntiClownApiBadRequestException(string message) : base(message)
    {
    }

    public override int StatusCode { get; set; } = 400;
}