using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    public GameObject[] toInstantiate;
    public float fieldRadius;
    public float nAsteroids;
    public float scaleNoise = 1.5f;

    private List<GameObject> instantiatedObjects;
    private Dictionary<int, CombineInstance> combinedInstances;

    private GameObject[] parents;
    private MeshFilter[] meshFilters;

    // Start is called before the first frame update
    void Start()
    {
        instantiatedObjects = new List<GameObject>();
        combinedInstances = new Dictionary<int, CombineInstance>();

        parents = new GameObject[toInstantiate.Length];
        meshFilters = new MeshFilter[toInstantiate.Length];
        
        // I create N parent objects, one per prefab type (material type)
        for (int i=0; i<parents.Length; i++)
        {
            // Giving it a meaningful name and parenting it to this object
            parents[i].name = toInstantiate[i].name + " field parent";
            parents[i].transform.parent = transform.parent;

            // Adding a meshfilter per object so I can combine them later
            meshFilters[i] = parents[i].AddComponent<MeshFilter>();
            meshFilters[i].mesh = new Mesh();
        }

        // Instantiating the asteroids
        StartCoroutine(InstantiateAsteroids());
        // Combining the meshes
        CombineMeshes();
        // Randomize the asteroid rotation
        RandomizeRotations();
    }

    private void RandomizeRotations()
    {
        Vector3 rot = new Vector3();

        for (int i=0; i<instantiatedObjects.Count; i++)
        {
            rot.x = Random.Range(0, 360);
            rot.y = Random.Range(0, 360);
            rot.z = Random.Range(0, 360);

            instantiatedObjects[i].transform.rotation = Quaternion.Euler(rot);
        }
    }

    private void CombineMeshes()
    {
        //CombineInstance[] instances = combinedInstances[0].
    }

    private IEnumerator InstantiateAsteroids()
    {
        Vector3 pos = new Vector3();

        for (int i=0; i<nAsteroids; i++)
        {
            int spawnIndex = Random.Range(0, toInstantiate.Length);
            pos.x = Random.Range(-fieldRadius, fieldRadius);
            pos.y = Random.Range(-fieldRadius, fieldRadius);
            pos.z = Random.Range(-fieldRadius, fieldRadius);

            GameObject asteroid = Instantiate(toInstantiate[spawnIndex], pos, Quaternion.Euler(Vector3.zero));
            MeshFilter asteroidFilter = asteroid.GetComponent<MeshFilter>();

            CombineInstance cm = new CombineInstance();
            cm.mesh = asteroid.GetComponent<MeshFilter>().sharedMesh;
            cm.transform = asteroidFilter.transform.localToWorldMatrix;

            combinedInstances.Add(spawnIndex, cm);
            instantiatedObjects.Add(asteroid);

            float scaleMul = Random.Range(1f, scaleNoise);

            asteroid.transform.localScale *= scaleMul;
        }

        Debug.Log("finished");

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
