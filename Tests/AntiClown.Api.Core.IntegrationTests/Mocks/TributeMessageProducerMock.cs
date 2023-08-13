using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Services;

namespace AntiClown.Api.Core.IntegrationTests.Mocks;

public class TributeMessageProducerMock : ITributeMessageProducer
{
    public Task ProduceAsync(Tribute tribute)
    {
        return Task.CompletedTask;
    }
}