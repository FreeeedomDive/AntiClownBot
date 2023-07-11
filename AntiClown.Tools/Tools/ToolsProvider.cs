namespace AntiClown.Tools.Tools;

public class ToolsProvider : IToolsProvider
{
    public ToolsProvider(IEnumerable<ToolBase> tools)
    {
        this.tools = tools.ToArray();
    }

    public ToolBase[] GetAll()
    {
        return tools;
    }

    public ToolBase? FindByName(string name)
    {
        return tools.FirstOrDefault(x => x.Name == name);
    }

    private readonly ToolBase[] tools;
}