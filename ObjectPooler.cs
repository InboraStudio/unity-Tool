using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool expandable = true;
    }

    [SerializeField] private List<Pool> pools;
    
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;
    private Dictionary<string, bool> expandableDictionary;
    private Dictionary<GameObject, string> objectToTagDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();
        expandableDictionary = new Dictionary<string, bool>();
        objectToTagDictionary = new Dictionary<GameObject, string>();

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            // Create a parent object for organization
            GameObject poolParent = new GameObject(pool.tag + "_Pool");
            poolParent.transform.SetParent(transform);

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolParent.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                objectToTagDictionary.Add(obj, pool.tag);
            }

            poolDictionary.Add(pool.tag, objectPool);
            prefabDictionary.Add(pool.tag, pool.prefab);
            expandableDictionary.Add(pool.tag, pool.expandable);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];

        // If the pool is empty and not expandable, return null
        if (objectPool.Count == 0 && !expandableDictionary[tag])
        {
            Debug.LogWarning("No more objects in pool " + tag + " and it's not expandable!");
            return null;
        }

        // If the pool is empty but expandable, create a new object
        if (objectPool.Count == 0)
        {
            GameObject newObj = Instantiate(prefabDictionary[tag], transform.Find(tag + "_Pool"));
            objectToTagDictionary.Add(newObj, tag);
            newObj.SetActive(false);
            objectPool.Enqueue(newObj);
        }

        GameObject objectToSpawn = objectPool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Call OnObjectSpawn method if the object has IPooledObject interface
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (!objectToTagDictionary.ContainsKey(obj))
        {
            Debug.LogWarning("This object wasn't created from a pool!");
            return;
        }

        string tag = objectToTagDictionary[obj];
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    public void ClearPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];
        while (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            Destroy(obj);
        }
    }

    public void ClearAllPools()
    {
        foreach (var pool in poolDictionary)
        {
            ClearPool(pool.Key);
        }
    }
}

// Interface for objects that need special initialization when spawned from pool
public interface IPooledObject
{
    void OnObjectSpawn();
} 