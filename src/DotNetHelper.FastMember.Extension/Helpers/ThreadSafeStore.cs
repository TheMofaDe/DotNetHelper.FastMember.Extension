using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DotNetHelper.FastMember.Extension.Extension;

internal class ThreadSafeStore<TKey, TValue>
{

    private readonly ConcurrentDictionary<TKey, TValue> _concurrentStore;

    private readonly Func<TKey, TValue> _creator;

    public ThreadSafeStore(Func<TKey, TValue> creator)
    {
        creator.IsNullThrow(nameof(creator));

        _creator = creator;
        _concurrentStore = new ConcurrentDictionary<TKey, TValue>();

    }

    public TValue Get(TKey key)
    {
        return _concurrentStore.GetOrAdd(key, _creator);
    }

}