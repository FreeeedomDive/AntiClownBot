namespace AntiClown.Core.Dto.Exceptions;

public abstract class AntiClownBaseException : Exception
{
    protected AntiClownBaseException(string message) : base(message)
    {
    }
    
    public abstract int StatusCode { get; set; }
}