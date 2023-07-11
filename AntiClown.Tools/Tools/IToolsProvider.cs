namespace AntiClown.Tools.Tools;

public interface IToolsProvider
{
    ToolBase[] GetAll();
    ToolBase? FindByName(string name);
}