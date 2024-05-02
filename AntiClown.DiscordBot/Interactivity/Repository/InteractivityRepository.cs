using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Utility.Locks;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public class InteractivityRepository : IInteractivityRepository
{
    public InteractivityRepository(
        ISqlRepository<InteractivityStorageElement> sqlRepository,
        ILocker locker
    )
    {
        this.sqlRepository = sqlRepository;
        this.locker = locker;
    }

    public async Task CreateAsync<T>(Interactivity<T> interactivity)
    {
        await locker.DoInLockAsync(
            GetLockId(interactivity.Id), () => sqlRepository.CreateAsync(
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
            )
        );
    }

    public async Task<Interactivity<T>?> TryReadAsync<T>(Guid id)
    {
        return await locker.ReadInLockAsync(
            GetLockId(id), async () =>
            {
                var result = await sqlRepository.TryReadAsync(id);
                return result is null ? null : ToModel<T>(result);
            }
        );
    }

    public async Task UpdateAsync<T>(Interactivity<T> interactivity)
    {
        await locker.DoInLockAsync(
            GetLockId(interactivity.Id), () => sqlRepository.UpdateAsync(
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
            )
        );
    }

    private static string GetLockId(Guid id) => $"Interactivity {id}";

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

    private readonly ISqlRepository<InteractivityStorageElement> sqlRepository;
    private readonly ILocker locker;
}