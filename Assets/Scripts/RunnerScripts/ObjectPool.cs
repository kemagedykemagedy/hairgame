using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    private GameObject objectPrefab;
    private Stack<GameObject> objPool = new Stack<GameObject>();

    public ObjectPool(GameObject prefab)
    {
        this.objectPrefab = prefab;
    }

    public void FillPool(int number,Transform poolParent)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject obj = Object.Instantiate(objectPrefab);
            obj.transform.SetParent(poolParent);
            obj.transform.GetComponent<HairCell>().PoolParent=poolParent;
            obj.transform.GetComponent<HairCell>().ResetColor();
            AddObjectToPool(obj);
        }
    }

    public GameObject PullFromPool()
    {
        if (objPool.Count > 0)
        {
            GameObject obje = objPool.Pop();
            obje.SetActive(true);
            //obje.GetComponent<HairCell>().AddForce();

            return obje;
        }

        return Object.Instantiate(objectPrefab);
    }

    public void PushObjectToPool(GameObject obje)
    {
        obje.GetComponent<HairCell>().ResetLevel();
        obje.GetComponent<HairCell>().ResetColor();
        AddObjectToPool(obje);
    }

    public void AddObjectToPool(GameObject obje)
    {
        obje.SetActive(false);
        objPool.Push(obje);
    }
}