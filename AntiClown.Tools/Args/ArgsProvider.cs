namespace AntiClown.Tools.Args;

public class ArgsProvider : IArgsProvider
{
    public ArgsProvider(string[] args)
    {
        argsByName = new Dictionary<string, string>();
        for (var i = 0; i < args.Length; i += 2)
        {
            argsByName[args[i]] = args[i + 1];
        }
    }

    public string GetArgument(string name)
    {
        return argsByName.TryGetValue(name, out var x) ? x : throw new ArgumentWasNotProvidedException(name);
    }

    public T GetArgument<T>(string name) where T : IParsable<T>
    {
        return T.TryParse(GetArgument(name), null, out var x) ? x : throw new WrongArgumentTypeException<T>(name);
    }

    private readonly Dictionary<string, string> argsByName;
}