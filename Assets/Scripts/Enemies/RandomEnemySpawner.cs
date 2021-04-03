using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    Rate, OnDeath
}

public class RandomEnemySpawner : MonoBehaviour
{
    [Header("Spawn behaviour")]
    public SpawnType spawnType;
    public float distanceFromPlayer;
    public float asteroidOffset = 10;
    public bool parent;

    [Header("Spawn rate data")]
    public float spawnRate;
    public float spawnRateNoise;

    [Header("Object references")]
    public GameObject[] toSpawn;
    public GameObject[] startObjects;

    private List<GameObject> currentObjects;


    // Start is called before the first frame update
    void Start()
    {
        currentObjects = new List<GameObject>();

        for (int i=0; i<startObjects.Length; i++)
        {
            startObjects[i].GetComponent<SpawnedEnemy>().SetSpawner(this, i);
            currentObjects.Add(startObjects[i]);
        }
        
        if (spawnType == SpawnType.Rate)
        {
            StartCoroutine(RateSpawn());
        }
    }

    private IEnumerator RateSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate + Random.Range(-spawnRateNoise, spawnRateNoise));

            currentObjects.Add(InstantiateEnemy());
        }
    }

    public void Destroyed(int destroyedIndex)
    {
        if (spawnType == SpawnType.OnDeath)
        {
            currentObjects[destroyedIndex] = InstantiateEnemy();
            currentObjects[destroyedIndex].GetComponent<SpawnedEnemy>().SetSpawner(this, destroyedIndex);
        }
    }

    private GameObject InstantiateEnemy()
    {
        float offsetX = Random.Range(-1, 1);
        float offsetY = Random.Range(-1, 1);
        float offsetZ = Random.Range(-1, 1);

        if (offsetX == 0)
            offsetX = 1;
        if (offsetY == 0)
            offsetY = -1;
        if (offsetZ == 0)
            offsetZ = 1;

        GameObject ret = Instantiate(toSpawn[Random.Range(0, toSpawn.Length)],
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * asteroidOffset +
            new Vector3(offsetX, offsetY, offsetZ) * distanceFromPlayer,
            Quaternion.Euler(Vector3.zero)
        );

        if (parent)
            ret.transform.parent = transform;

        return ret;
    }
}
