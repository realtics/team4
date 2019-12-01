using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolFactory
{
    List<RecycleObject> _pool = new List<RecycleObject>();
    int _defaultPoolSize;
    RecycleObject _prefab;

    public PoolFactory(RecycleObject prefab, int defaultPoolSize = 5)
    {
        this._prefab = prefab;
        this._defaultPoolSize = defaultPoolSize;

        Debug.Assert(this._prefab != null, "Prefab is null!");
    }

    void CreatePool()
    {
        for (int i = 0; i < _defaultPoolSize; ++i)
        {
            RecycleObject obj = GameObject.Instantiate(_prefab) as RecycleObject;
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
        }
    }

    public RecycleObject Get()
    {
        if (_pool.Count == 0)
        {
            CreatePool();
        }

        int lastIndex = _pool.Count - 1;
        RecycleObject obj = _pool[lastIndex];
        _pool.RemoveAt(lastIndex);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Restore(RecycleObject obj)
    {
        Debug.Assert(obj != null, "NUll object to be returned!");
        obj.gameObject.SetActive(false);
        _pool.Add(obj);
    }
}
