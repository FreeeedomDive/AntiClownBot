namespace AntiClown.DiscordBot.Utility.Locks;

public static class LockerExtensions
{
    public static async Task<T> ReadInLockAsync<T>(this ILocker locker, string lockKey, Func<Task<T>> func)
    {
        await locker.AcquireAsync(lockKey);
        var result = await func();
        await locker.ReleaseAsync(lockKey);
        return result;
    }

    public static async Task DoInLockAsync(this ILocker locker, string lockKey, Func<Task> action)
    {
        await locker.AcquireAsync(lockKey);
        await action();
        await locker.ReleaseAsync(lockKey);
    }
}