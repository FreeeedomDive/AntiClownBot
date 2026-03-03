using AntiClown.Tools.Utility.Random;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Utility.Locks;

public class Locker : ILocker
{
    public Locker(ISqlRepository<LockStorageElement> sqlRepository, ILogger<Locker> logger)
    {
        this.sqlRepository = sqlRepository;
        this.logger = logger;
    }

    public async Task AcquireAsync(string key)
    {
        var lockExists = true;
        while (lockExists)
        {
            var @lock = await FindAsync(key);
            lockExists = @lock is not null;
            if (!lockExists)
            {
                continue;
            }

            var sleepTime = Randomizer.GetRandomNumberBetween(100, 1000);
            logger.LogInformation("Lock {key} is acquired, waiting for {ms}ms", key, sleepTime);
            await Task.Delay(sleepTime);
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
    private readonly ILogger<Locker> logger;
}