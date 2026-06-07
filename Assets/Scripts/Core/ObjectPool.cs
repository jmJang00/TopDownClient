using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Stack<T> _stack = new Stack<T>();

    private readonly T _prefab;
    private readonly Transform _root;

    public ObjectPool(T prefab, int initialCount = 0, Transform root = null)
    {
        _prefab = prefab;
        _root = root;

        for (int i = 0; i < initialCount; ++i)
        {
            T obj = Create();
            obj.gameObject.SetActive(false);
            _stack.Push(obj);
        }
    }

    private T Create()
    {
        T obj = Object.Instantiate(_prefab, _root);
        return obj;
    }

    public T Acquire()
    {
        if (_stack.Count > 0)
        {
            return _stack.Pop();
        }

        return Create();
    }

    public void Release(T obj)
    {
        _stack.Push(obj);
    }

    public void Clear()
    {
        while (_stack.Count > 0)
        {
            T obj = _stack.Pop();

            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }
    }

    public int Count
    {
        get { return _stack.Count; }
    }
}
