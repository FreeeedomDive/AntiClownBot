namespace AntiClown.Tools.Tools;

public class ToolsProvider : IToolsProvider
{
    public ToolsProvider(IEnumerable<ITool> tools)
    {
        this.tools = tools.ToArray();
    }

    public ITool[] GetAll()
    {
        return tools;
    }

    public ITool? FindByName(string name)
    {
        return tools.FirstOrDefault(x => x.Name == name);
    }

    private readonly ITool[] tools;
}