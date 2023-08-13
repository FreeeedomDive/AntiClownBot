using AntiClown.Entertainment.Api.Core.Parties.Domain;

namespace AntiClown.Entertainment.Api.Core.Parties.Services.Messages;

public interface IPartiesMessageProducer
{
    Task ProduceAsync(Party party);
}