namespace Launcher.Processes
{
    public interface IProcess
    {
        public string Name { get; }
        public void Start();
    }
}