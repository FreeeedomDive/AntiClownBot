namespace AntiClown.Core.Dto.Exceptions;

public abstract class AntiClownBaseException : Exception
{
    protected AntiClownBaseException(string message) : base(message)
    {
    }

    protected AntiClownBaseException(string message, Exception baseException) : base(message, baseException)
    {
    }
    
    public abstract int StatusCode { get; }
}