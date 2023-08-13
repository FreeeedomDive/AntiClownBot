namespace AntiClown.Tools.Args;

public interface IArgsProvider
{
    string GetArgument(string name);
    T GetArgument<T>(string name) where T : IParsable<T>;
}