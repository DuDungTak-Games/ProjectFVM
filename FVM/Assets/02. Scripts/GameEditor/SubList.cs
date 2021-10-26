using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubList<T> : List<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<T> datas = new List<T>();

    public void OnBeforeSerialize()
    {
        datas.Clear();

        foreach (T data in this)
        {
            datas.Add(data);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0, icount = datas.Count; i < icount; ++i)
        {
            this.Add(datas[i]);
        }
    }

    public void Clear()
    {
        datas.Clear();
    }
}