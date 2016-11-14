using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FixedSizedQueue<T>
{
  protected Queue<T> q = new Queue<T>();

  public int MaxSize { get; set; }

  public FixedSizedQueue (int maxSize)
  {
    MaxSize = maxSize;
  }

  public T Peek ()
  {
    return q.Peek ();
  }

  public void Clear ()
  {
    q.Clear ();
  }

  public void Enqueue(T obj)
  {
    if (q.Count == MaxSize)
    {
      q.Dequeue ();
    }

    q.Enqueue (obj);
  }
}

public class FixedSizedFloatQueue : FixedSizedQueue<float>
{

  public FixedSizedFloatQueue (int maxSize) : base (maxSize)
  {
  }

  public float Average ()
  {
    float sum = 0f;
    foreach (float f in q)
    {
      sum += f;
    }

    return q.Count == 0 ? 0f : sum / q.Count;
  }
}

public class FixedSizedVector2Queue : FixedSizedQueue<Vector2>
{

  public FixedSizedVector2Queue (int maxSize) : base (maxSize)
  {
  }

  public Vector2 Average ()
  {
    Vector2 sum = new Vector2 (0f, 0f);
    foreach (Vector2 v in q)
    {
      sum += v;
    }

    return q.Count == 0 ? new Vector2 (0f,0f) : sum / q.Count;
  }
}