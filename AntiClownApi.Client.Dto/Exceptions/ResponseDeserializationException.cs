namespace AntiClownApiClient.Dto.Exceptions;

public class ResponseDeserializationException : Exception
{
    public ResponseDeserializationException(string message) : base(message)
    {
        
    }
}