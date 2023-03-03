namespace AntiClown.Api.Dto.Exceptions.Base;

public abstract class AntiClownApiBaseException : Exception
{
    protected AntiClownApiBaseException(string message) : base(message)
    {
    }
    
    public abstract int StatusCode { get; set; }
}