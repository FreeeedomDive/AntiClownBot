namespace AntiClown.Tools.Args;

public class WrongArgumentTypeException<T> : Exception
{
    public WrongArgumentTypeException(string argumentName)
        : base($"Argument with name {argumentName} can not be parsed as {typeof(T).Name}")
    {
    }
}