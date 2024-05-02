using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Utility.Locks;

public class Locker : ILocker
{
    public Locker(ISqlRepository<LockStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task AcquireAsync(string key)
    {
        var lockExists = true;
        while (lockExists)
        {
            var @lock = await FindAsync(key);
            lockExists = @lock is not null;
        }

        await sqlRepository.CreateAsync(
            new LockStorageElement
            {
                Id = Guid.NewGuid(),
                Key = key,
            }
        );
    }

    public async Task ReleaseAsync(string key)
    {
        var @lock = await FindAsync(key);
        if (@lock is null)
        {
            return;
        }

        await sqlRepository.DeleteAsync(@lock.Id);
    }

    private async Task<LockStorageElement?> FindAsync(string key)
    {
        return (await sqlRepository.FindAsync(x => x.Key == key)).FirstOrDefault();
    }

    private readonly ISqlRepository<LockStorageElement> sqlRepository;
}