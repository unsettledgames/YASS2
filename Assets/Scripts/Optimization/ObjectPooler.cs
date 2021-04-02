using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : OptimizedMonoBehaviour
{
    [System.Serializable]
    public class PoolObject
    {
        public string name;
        public int amount;
        public GameObject reference;
    }

    public static ObjectPooler Instance;
    public PoolObject[] pooledObjects;

    public int genericTypesAmount;

    private Dictionary<string, Queue<GameObject>> objects;
    private GameObject objectParent;
    private Queue<Vector3> vector3Queue;
    private Queue<Vector2> vector2Queue;
    private Queue<Color> colorQueue;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        objects = new Dictionary<string, Queue<GameObject>>();

        objectParent = new GameObject();
        vector3Queue = new Queue<Vector3>();
        vector2Queue = new Queue<Vector2>();
        colorQueue = new Queue<Color>();

        for (int i=0; i<pooledObjects.Length; i++)
        {
            objects.Add(pooledObjects[i].name, new Queue<GameObject>());

            for (int j=0; j<pooledObjects[i].amount; j++)
            {
                GameObject toAdd = Instantiate(pooledObjects[i].reference, Vector3.zero, Quaternion.identity);
                toAdd.SetActive(false);
                toAdd.transform.parent = objectParent.transform;

                objects[pooledObjects[i].name].Enqueue(toAdd);
            }
        }

        objects.Add("EmptyGameObject", new Queue<GameObject>());
        for (int i=0; i<genericTypesAmount; i++)
        {
            GameObject toEnqueue = new GameObject();
            toEnqueue.transform.parent = objectParent.transform;
            toEnqueue.SetActive(false);

            objects["EmptyGameObject"].Enqueue(toEnqueue);
        }

        for (int i=0; i<genericTypesAmount; i++)
        {
            vector3Queue.Enqueue(new Vector3());
            vector2Queue.Enqueue(new Vector2());
            colorQueue.Enqueue(new Color());
        }
    }
    private void Update()
    {
        if (vector3Queue.Count == 0)
        {
            vector3Queue.Enqueue(new Vector3());
        }

        if (vector2Queue.Count == 0)
        {
            vector2Queue.Enqueue(new Vector2());
        }
    }

    public GameObject Dequeue(Vector3 position, Quaternion rotation, string name)
    {
        GameObject ret = objects[name].Dequeue();

        if (objects[name].Count == 0)
        {
            return null;
        }
        else
        {
            ret.SetActive(true);
            ret.transform.parent = null;
            ret.transform.position = position;
            ret.transform.rotation = rotation;

            return ret;
        }
    }

    public void Enqueue(GameObject toAdd, string name)
    {
        if (toAdd == null && objects[name].Count < genericTypesAmount)
        {
            toAdd = new GameObject();
        }
        else
        {
            StartCoroutine(RemoveComponents(toAdd));
        }

        toAdd.SetActive(false);
        toAdd.name = "Enqueued GameObject";
        toAdd.transform.parent = objectParent.transform;

        objects[name].Enqueue(toAdd);
    }

    public Color GetColor()
    {
        return colorQueue.Dequeue();
    }
    public void EnqueueColor(Color toAdd)
    {
        colorQueue.Enqueue(toAdd);
    }

    public Vector3 GetVector3()
    {
        return vector3Queue.Dequeue();
    }

    public void EnqueueVector3(Vector3 toAdd)
    {
        if (vector3Queue.Count < genericTypesAmount)
            vector3Queue.Enqueue(toAdd);
    }

    public Vector2 GetVector2()
    {
        return vector2Queue.Dequeue();
    }

    public void EnqueueVector2(Vector2 toAdd)
    {
        if (vector2Queue.Count < genericTypesAmount)
            vector2Queue.Enqueue(toAdd);
    }

    public IEnumerator TimeEnqueueGameObject(GameObject reference, string name, float toWait)
    {
        yield return new WaitForSecondsRealtime(toWait);
        
        Enqueue(reference, name);
    }
    
    private IEnumerator RemoveComponents(GameObject go)
    {
        Component[] components = go.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (!(comp is Transform))
            {
                Destroy(comp);
                yield return null;
            }
        }
    }
}