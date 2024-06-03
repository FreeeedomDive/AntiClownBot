using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class DriverNotFoundException : NotFoundException
{
    public DriverNotFoundException(string name) : base($"Driver with name {name} not found")
    {
        Name = name;
    }

    public string Name { get; }
}