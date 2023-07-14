using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class DriverNotFoundException : AntiClownNotFoundException
{
    public DriverNotFoundException(string name) : base($"Driver with name {name} not found")
    {
        Name = name;
    }

    public string Name { get; }
}