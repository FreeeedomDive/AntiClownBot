namespace AntiClown.Tools.Args;

public class ArgumentWasNotProvidedException : Exception
{
    public ArgumentWasNotProvidedException(string argumentName)
        : base($"Argument with name {argumentName} was not provided")
    {
    }
}