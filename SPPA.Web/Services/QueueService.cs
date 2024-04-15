namespace SPPA.Web.Services;

public static class QueueService
{
    private const int UserLimit = 2;
    private const int GlobalLimit = 6;

    private static readonly object _lockObject = new();

    private static readonly Semaphore _globalLock = new(GlobalLimit, GlobalLimit);
    private static readonly Dictionary<string, SemaphoreWithCount> _userLock = new();

    public static void Wait(string userId)
    {
        SemaphoreWithCount currentUserLock;

        lock (_lockObject)
        {
            if (_userLock.ContainsKey(userId))
                currentUserLock = _userLock[userId];
            else
            {
                currentUserLock = new(UserLimit);
                _userLock.Add(userId, currentUserLock);
            }
            currentUserLock.Count++;
        }

        currentUserLock.WaitOne();
        _globalLock.WaitOne();
    }

    public static void Release(string userId)
    {
        _globalLock.Release();
        lock (_lockObject)
        {
            var currentUserLock = _userLock[userId];
            currentUserLock.Release();
            currentUserLock.Count--;
            if (currentUserLock.Count == 0)
            {
                _userLock.Remove(userId);
                currentUserLock.Dispose();
            }
        }
    }


    private class SemaphoreWithCount : IDisposable
    {
        private readonly Semaphore _semaphore;
        public int Count;

        public SemaphoreWithCount(int count)
        {
            _semaphore = new Semaphore(count, count);
            Count = 0;
        }

        public void WaitOne()
        {
            this._semaphore.WaitOne();
        }

        public void Release()
        {
            this._semaphore.Release();
        }

        public void Dispose()
        {
            this._semaphore.Dispose();
        }
    }

}

