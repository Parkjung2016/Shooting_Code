using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PoolManager
{
    public static PoolManager Instance;

    private Dictionary<string, Pool<PoolableMono>> _pools
                            = new Dictionary<string, Pool<PoolableMono>>();
    private Transform _trmParent;

    public PoolManager(Transform trmParent)
    {
        _trmParent = trmParent;
    }

    public void CreatePool(PoolableMono prefab, int count = 10)
    {
        Pool<PoolableMono> pool = new Pool<PoolableMono>(prefab, _trmParent, count);
        _pools.Add(prefab.gameObject.name, pool);
    }

    public PoolableMono Pop(string name)
    {

        if (!_pools.ContainsKey(name))
        {
            Debug.LogError($"Prefab does not exist on pool : {name}");
            return null;
        }
        PoolableMono item = _pools[name].Pop();
        item.Init();
        return item;
    }
    public int EnemyNum()
    {
        int num = 0;
        foreach(KeyValuePair<string,Pool<PoolableMono>> pool in _pools)
        {
            if (pool.Key.Contains("Enemy"))
                num++;
        }
        return num;
    }
    public int ObstacleNum()
    {
        int num = 0;
        foreach (KeyValuePair<string, Pool<PoolableMono>> pool in _pools)
        {
            if (pool.Key.Contains("Obstacle"))
                num++;
        }
        return num;
    }
    public void Push(PoolableMono obj)
    {
        _pools[obj.name].Push(obj);
    }

}