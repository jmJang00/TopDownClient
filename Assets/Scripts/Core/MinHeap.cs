using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private readonly List<T> _data = new List<T>(128);

    public int Count => _data.Count;

    public void Clear()
    {
        _data.Clear();
    }

    public void Push(T item)
    {
        _data.Add(item);
        int i = _data.Count - 1;

        while (i > 0)
        {
            int parent = (i - 1) >> 1;
            if (_data[parent].CompareTo(_data[i]) <= 0)
                break;

            Swap(i, parent);
            i = parent;
        }
    }

    public T Peek()
    {
        return _data[0];
    }

    public T Pop()
    {
        int last = _data.Count - 1;
        T root = _data[0];

        _data[0] = _data[last];
        _data.RemoveAt(last);

        Heapify(0);
        return root;
    }

    private void Heapify(int i)
    {
        int count = _data.Count;

        while (true)
        {
            int left = (i << 1) + 1;
            int right = left + 1;
            int smallest = i;

            if (left < count && _data[left].CompareTo(_data[smallest]) < 0)
                smallest = left;

            if (right < count && _data[right].CompareTo(_data[smallest]) < 0)
                smallest = right;

            if (smallest == i)
                break;

            Swap(i, smallest);
            i = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        T tmp = _data[a];
        _data[a] = _data[b];
        _data[b] = tmp;
    }
}
