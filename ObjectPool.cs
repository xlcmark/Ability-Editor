using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>:MonoBehaviour where T:MonoBehaviour
{
    public Dictionary<T, List<T>> pools = new Dictionary<T, List<T>>();
    private T CreateObject(T _prefab,List<T> pool)
    {
        T newOne = Instantiate(_prefab);
        pool.Add(newOne);
        newOne.gameObject.SetActive(false);
        return newOne;
    }
    public T GetObject(T _prefab)
    {
        T projectile=null;
        List<T> pool;
        if(pools.TryGetValue(_prefab,out pool))
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].gameObject.activeInHierarchy)
                {
                    projectile = pool[i];
                    break;
                }
            }
        }
        else//還沒建立pool
        {
            pool = CreatePool(_prefab, 1);
        }
        if (projectile == null)//若無物件可用，創造一個新的
        {
            projectile=CreateObject(_prefab, pool);
        }
        projectile.gameObject.SetActive(true);
        return projectile;
    }
    public List<T> CreatePool(T _prefab,int _count)
    {
        List<T> pool;
        if (pools.TryGetValue(_prefab, out pool))
        {
            for (int i = 0; i < _count; i++)
            {
                CreateObject(_prefab, pool);
            }
        }
        else
        {
            pool = new List<T>();
            for (int i = 0; i < _count; i++)
            {
                CreateObject(_prefab, pool);
            }
            pools.Add(_prefab, pool);
        }
        return pool;
    }
}
