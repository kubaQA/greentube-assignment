namespace PetStore.Tests;

public static class TestUtils
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(8);
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(500);

    // Boolean condition with defaults
    public static Task<bool> RetryUntilAsync(Func<Task<bool>> condition) =>
        RetryUntilAsync(condition, DefaultTimeout, DefaultInterval);

    // Generic with defaults
    public static Task<T?> RetryUntilAsync<T>(Func<Task<T?>> action) where T : class =>
        RetryUntilAsync(action, DefaultTimeout, DefaultInterval);

    // Existing overloads with explicit timeout/interval
    public static async Task<bool> RetryUntilAsync(
        Func<Task<bool>> condition,
        TimeSpan timeout,
        TimeSpan interval)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            if (await condition()) return true;
            await Task.Delay(interval);
        }
        return false;
    }

    public static async Task<T?> RetryUntilAsync<T>(
        Func<Task<T?>> action,
        TimeSpan timeout,
        TimeSpan interval) where T : class
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            var result = await action();
            if (result != null) return result;
            await Task.Delay(interval);
        }
        return null;
    }
}