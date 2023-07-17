using AntiClown.DiscordBot.Interactivity.Domain;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public class InteractivityRepository : IInteractivityRepository
{
    public InteractivityRepository(ISqlRepository<InteractivityStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(Domain.Interactivity interactivity)
    {
        await sqlRepository.CreateAsync(
            new InteractivityStorageElement
            {
                Id = interactivity.Id,
                Type = interactivity.Type.ToString(),
                MessageId = interactivity.MessageId,
                AuthorId = interactivity.AuthorId,
            }
        );
    }

    public async Task<Domain.Interactivity> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return new Domain.Interactivity
        {
            Id = result.Id,
            Type = Enum.TryParse<InteractivityType>(result.Type, out var interactivityType)
                ? interactivityType
                : throw new NotSupportedException($"Unknown interactivity type {result.Type}"),
            MessageId = result.MessageId,
            AuthorId = result.AuthorId,
        };
    }

    private readonly ISqlRepository<InteractivityStorageElement> sqlRepository;
}