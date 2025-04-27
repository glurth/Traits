using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SerializableNestedList<T>:IList<T>
{
    public List<T> internalList;

    public T this[int index] { get => ((IList<T>)internalList)[index]; set => ((IList<T>)internalList)[index] = value; }

    public int Count => ((ICollection<T>)internalList).Count;

    public bool IsReadOnly => ((ICollection<T>)internalList).IsReadOnly;

    public void Add(T item)
    {
        ((ICollection<T>)internalList).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)internalList).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)internalList).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)internalList).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)internalList).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ((IList<T>)internalList).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<T>)internalList).Insert(index, item);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)internalList).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<T>)internalList).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)internalList).GetEnumerator();
    }

    public static implicit operator List<T>(SerializableNestedList<T> wrapper) => wrapper.internalList;
    public static implicit operator SerializableNestedList<T>(List<T> list) => new SerializableNestedList<T> { internalList = list };
}
