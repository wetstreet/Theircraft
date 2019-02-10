using System.Collections.Generic;
using UnityEngine;

class GameObjectPool
{
    static int poolInitSize = 1000;
    static int poolMaxSize = 10000;
    
    Transform transform;
    GameObject item;
    Queue<GameObject> pool = new Queue<GameObject>();

    public GameObjectPool(GameObject _item, string name)
    {
        item = _item;
        transform = new GameObject(name).transform;

        for (int i = 0; i < poolInitSize; i++)
        {
            InstantiateItem();
        }
    }

    public void InstantiateItem()
    {
        GameObject obj = Object.Instantiate(item);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        pool.Enqueue(obj);
    }

    public GameObject GetItem()
    {
        if (pool.Count == 0)
            InstantiateItem();
        return pool.Dequeue();
    }

    public void Recycle(GameObject obj)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        pool.Enqueue(obj);
    }
}
