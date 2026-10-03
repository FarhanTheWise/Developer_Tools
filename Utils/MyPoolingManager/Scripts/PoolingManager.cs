using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    
    #region All Variables
    public static PoolingManager instance;
    
    [Header("General Settings")]
    public bool startPool;
    public List<PoolObject> poolContainers;

    [Header("Display Pools Settings")]    
    public List<PoolParent> poolParents;
    private Dictionary<string, Queue<GameObject>> poolDictionary;


    #endregion

    #region Start Method

    public void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pools in poolContainers)
        {

            if(!pools.OnAwake) continue;
            var newChild = Instantiate(pools.objectGroup);
            newChild.name = pools.poolType.ToString();
            newChild.transform.parent = transform;
            var objectPool = new Queue<GameObject>();

            for (var i = 0; i < pools.size; i++)
            {

                var go = Instantiate(pools.prefab);
                go.name = $"{pools.poolTitle}_{i}";
                go.transform.parent = newChild.transform;


                go.SetActive(false);
                objectPool.Enqueue(go);

            }

            poolParents.Add(new PoolParent
            {
                poolName = pools.poolType,
                poolParent = newChild.transform
            });

            poolDictionary[pools.poolType.ToString()] = objectPool;

        }
    }

    public void InitializePool(PoolName pool)
    {
        var poolObject = poolContainers.FirstOrDefault(value => value.poolType.Equals(pool));
        if(poolObject == null)
        {
            Debug.Log("Invalid Pool Name");
            return;
        }

        var newChild = Instantiate(poolObject.objectGroup);
        newChild.name = poolObject.poolType.ToString();
        newChild.transform.parent = transform;
        var objectPool = new Queue<GameObject>();

        for (var i = 0; i < poolObject.size; i++)
        {

            var go = Instantiate(poolObject.prefab);
            go.name = $"{poolObject.poolTitle}_{i}";
            go.transform.parent = newChild.transform;


            go.SetActive(false);
            objectPool.Enqueue(go);

        }

        poolDictionary[poolObject.poolType.ToString()] = objectPool;
    }

    #endregion
    
    #region Spawn pooled object
    public T GetSpawnObject<T>(PoolName poolType, Vector3 position, Quaternion rotation, Transform setParent = null)
    {

        if (!startPool)
        {
            return default;
        }
        
        var go = poolDictionary[poolType.ToString()].Dequeue();

        go.transform.position = Vector3.zero;

        go.transform.SetPositionAndRotation(position, rotation);
        poolDictionary[poolType.ToString()].Enqueue(go);

        if(setParent != null)
            go.transform.SetParent(setParent);

        if (typeof(T).Equals(typeof(GameObject)))
            return (T)(object)go;

        var component = go.GetComponent<T>();

        return component;
    }

    public void ReturnToPool(PoolName poolName, GameObject poolObject)
    {
        poolObject.SetActive(false);
        poolObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        var poolLocation = poolParents.FirstOrDefault(value => value.poolName.Equals(poolName));
        if(poolLocation == null)
        {
            Debug.Log("Invalid Pool Type");
            return;
        }

        poolObject.transform.SetParent(poolLocation.poolParent);
    }

    #endregion

    #region Reset Pools

    public void ResetPools()
    {
        foreach (var pools in poolDictionary.Values)
        {
            foreach (var pool in pools)
            {
                pool.SetActive(false);
            }
        }
    }

    #endregion

}

[Serializable]
public class PoolParent
{
    public PoolName poolName;
    public Transform poolParent;
}
