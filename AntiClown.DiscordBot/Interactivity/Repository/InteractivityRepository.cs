using AntiClown.DiscordBot.Interactivity.Domain;
using Medallion.Threading;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public class InteractivityRepository : IInteractivityRepository
{
    public InteractivityRepository(
        ISqlRepository<InteractivityStorageElement> sqlRepository,
        IDistributedLockProvider distributedLockProvider
    )
    {
        this.sqlRepository = sqlRepository;
        this.distributedLockProvider = distributedLockProvider;
    }

    public async Task CreateAsync<T>(Interactivity<T> interactivity)
    {
        await using (await distributedLockProvider.AcquireLockAsync($"Interactivity {interactivity.Id}"))
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
    }

    public async Task<Interactivity<T>?> TryReadAsync<T>(Guid id)
    {
        await using (await distributedLockProvider.AcquireLockAsync($"Interactivity {id}"))
        {
            var result = await sqlRepository.TryReadAsync(id);
            return result == null ? null : ToModel<T>(result);
        }
    }

    public async Task<Interactivity<T>[]> FindByTypeAsync<T>(InteractivityType interactivityType)
    {
        var result = await sqlRepository.FindAsync(x => x.Type == interactivityType.ToString());
        return result.Select(ToModel<T>).ToArray();
    }

    public async Task UpdateAsync<T>(Interactivity<T> interactivity)
    {
        await using (await distributedLockProvider.AcquireLockAsync($"Interactivity {interactivity.Id}"))
        {
            await sqlRepository.UpdateAsync(
                interactivity.Id, x =>
                {
                    x.MessageId = interactivity.MessageId;
                    x.Type = interactivity.Type.ToString();
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
    }

    private static Interactivity<T> ToModel<T>(InteractivityStorageElement storageElement)
    {
        return new Interactivity<T>
        {
            Id = storageElement.Id,
            Type = Enum.TryParse<InteractivityType>(storageElement.Type, out var interactivityType)
                ? interactivityType
                : throw new NotSupportedException($"Unknown interactivity type {storageElement.Type}"),
            MessageId = storageElement.MessageId,
            AuthorId = storageElement.AuthorId,
            Details = string.IsNullOrEmpty(storageElement.Details) ? default : JsonConvert.DeserializeObject<T>(storageElement.Details),
            CreatedAt = storageElement.CreatedAt,
        };
    }

    private readonly IDistributedLockProvider distributedLockProvider;
    private readonly ISqlRepository<InteractivityStorageElement> sqlRepository;
}