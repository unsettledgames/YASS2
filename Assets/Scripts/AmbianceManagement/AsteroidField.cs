using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    public GameObject[] toInstantiate;
    public float instantiateDistance;
    public float fieldRadius;
    public float nAsteroids;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InstantiateAsteroids());
    }

    private IEnumerator InstantiateAsteroids()
    {
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();

        for (int i=0; i<nAsteroids; i++)
        {
            pos.x = Random.Range(-fieldRadius, fieldRadius);
            pos.y = Random.Range(-fieldRadius, fieldRadius);
            pos.z = Random.Range(-fieldRadius, fieldRadius);

            rot.x = Random.Range(0, 360);
            rot.y = Random.Range(0, 360);
            rot.z = Random.Range(0, 360);

            Instantiate(toInstantiate[Random.Range(0, toInstantiate.Length)], pos, Quaternion.Euler(rot));

            Debug.Log(i);

            yield return null;
        }

        Debug.Log("finished");

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
