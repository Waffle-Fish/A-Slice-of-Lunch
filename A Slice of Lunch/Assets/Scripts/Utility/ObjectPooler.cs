using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToCopy;

    [SerializeField]
    private int maxObjectCount = 1;
    [SerializeField]
    private Transform parent;

    private List<GameObject> objectPool;

    private void Awake() {
        if (parent == null) parent = this.transform;
        objectPool = new();
        GameObject tmp;
        for (int i = 0; i < maxObjectCount; i++)
        {
            tmp = Instantiate(objectToCopy,parent);
            tmp.SetActive(false);
            objectPool.Add(tmp);
        }
    }

    /// <summary> Returns the earliest inactive game object. Returns null if all objects are active 
    /// </summary> 
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        for(int i = 0; i < maxObjectCount; i++)
        {
            if(!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }
        return null;
    }

    public GameObject GetPooledObjectAtIndex(int index)
    {
        if(index >= objectPool.Count) throw new ArgumentOutOfRangeException("Parameter index is out of range.");
        return objectPool[index];
    }

    public void SetObjectToCopy(GameObject g) {
        objectToCopy = g;
    }

    public int GetObjectPoolCount() {
        return objectPool.Count;
    }

    public int GetNumObjectsActive() {
        int count = 0;
        while(objectPool[count].activeSelf) count++;
        return count;
    }
}
