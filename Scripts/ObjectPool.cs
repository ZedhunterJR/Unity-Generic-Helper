using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic object pool that can work with any type (Unity objects or plain classes).
/// </summary>
public class ObjectPool<T>
{
    private readonly Func<T> createFunc;         // How to create new instances
    private readonly Action<T> onGet;            // What to do when object is taken from pool
    private readonly Action<T> onRelease;        // What to do when object is returned
    private readonly Queue<T> pool;

    public int CountInactive => pool.Count;

    public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 5)
    {
        this.createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        this.onGet = onGet;
        this.onRelease = onRelease;
        pool = new Queue<T>(initialSize);

        // Pre-populate pool
        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(createFunc());
    }

    /// <summary>
    /// Get an object from the pool.
    /// </summary>
    public T Get()
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : createFunc();
        onGet?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Return an object to the pool.
    /// </summary>
    public void Release(T obj)
    {
        onRelease?.Invoke(obj);
        pool.Enqueue(obj);
    }
}

