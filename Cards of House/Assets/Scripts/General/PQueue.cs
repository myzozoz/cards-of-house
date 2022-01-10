using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQueue<TElement> where TElement: System.IEquatable<TElement> 
{
    private List<(TElement, float)> arr;

    public PQueue()
    {
        arr = new List<(TElement, float)>();
    }

    public void Enqueue(TElement e, float p)
    {
        int i = 0;
        while (i < arr.Count && arr[i].Item2 <= p)
        {
            i++;
        }
        arr.Insert(i, (e, p));
    }

    public TElement Dequeue()
    {
        if (arr.Count == 0)
        {
            return default(TElement);
        }

        TElement val = arr[0].Item1;
        arr.RemoveAt(0);
        return val;
    }

    public TElement Peek()
    {
        if (arr.Count == 0)
        {
            return default(TElement);
        }

        TElement val = arr[0].Item1;
        return val;
    }

    public void Clear()
    {
        arr.Clear();
    }

    public int GetCount()
    {
        return arr.Count;
    }

    public bool Contains(TElement e)
    {
        foreach ((TElement, float) x in arr)
        {
            if (x.Item1.Equals(e))
            {
                return true;
            }
        }
        return false;
    }

    public void Print()
    {
        Debug.Log("Printing priority queue contents:");
        foreach ((TElement, float) x in arr)
        {
            Debug.Log("\t" + x.Item1 + "\t(" + x.Item2 + ")");
        }
    }
}