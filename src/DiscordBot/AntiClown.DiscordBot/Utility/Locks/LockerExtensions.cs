namespace AntiClown.DiscordBot.Utility.Locks;

public static class LockerExtensions
{
    public static async Task<T> ReadInLockAsync<T>(this ILocker locker, string lockKey, Func<Task<T>> func)
    {
        try
        {
            await locker.AcquireAsync(lockKey);
            return await func();
        }
        finally
        {
            await locker.ReleaseAsync(lockKey);
        }
    }

    public static async Task DoInLockAsync(this ILocker locker, string lockKey, Func<Task> action)
    {
        try
        {
            await locker.AcquireAsync(lockKey);
            await action();
        }
        finally
        {
            await locker.ReleaseAsync(lockKey);
        }
    }
}