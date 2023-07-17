using AntiClown.DiscordBot.Interactivity.Domain;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public class InteractivityRepository : IInteractivityRepository
{
    public InteractivityRepository(ISqlRepository<InteractivityStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync<T>(Interactivity<T> interactivity)
    {
        await sqlRepository.CreateAsync(
            new InteractivityStorageElement
            {
                Id = interactivity.Id,
                Type = interactivity.Type.ToString(),
                MessageId = interactivity.MessageId,
                AuthorId = interactivity.AuthorId,
                Details = interactivity.Details is not null
                    ? JsonConvert.SerializeObject(
                        interactivity.Details, Formatting.Indented, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                        }
                    )
                    : string.Empty,
                CreatedAt = DateTime.UtcNow,
            }
        );
    }

    public async Task<Interactivity<T>?> TryReadAsync<T>(Guid id)
    {
        var result = await sqlRepository.TryReadAsync(id);
        if (result == null)
        {
            return null;
        }

        return new Interactivity<T>
        {
            Id = result.Id,
            Type = Enum.TryParse<InteractivityType>(result.Type, out var interactivityType)
                ? interactivityType
                : throw new NotSupportedException($"Unknown interactivity type {result.Type}"),
            MessageId = result.MessageId,
            AuthorId = result.AuthorId,
            Details = string.IsNullOrEmpty(result.Details) ? default : JsonConvert.DeserializeObject<T>(result.Details),
            CreatedAt = result.CreatedAt,
        };
    }

    public async Task UpdateAsync<T>(Interactivity<T> interactivity)
    {
        await sqlRepository.UpdateAsync(
            interactivity.Id, x =>
            {
                x.Details = interactivity.Details is not null
                    ? JsonConvert.SerializeObject(
                        interactivity.Details, Formatting.Indented, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                        }
                    )
                    : string.Empty;
            }
        );
    }

    private readonly ISqlRepository<InteractivityStorageElement> sqlRepository;
}