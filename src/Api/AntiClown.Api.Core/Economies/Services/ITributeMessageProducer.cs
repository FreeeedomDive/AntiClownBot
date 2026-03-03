using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface ITributeMessageProducer
{
    Task ProduceAsync(Tribute tribute);
}