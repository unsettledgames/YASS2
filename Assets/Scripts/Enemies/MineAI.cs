using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MineAI : OptimizedMonoBehaviour
{
    // Radius in which the mine explodes and gives damage
    public float explodeRadius;
    // Time you have to stay in the area to make the mine explode
    public float explodeTime;
    // Speed of light expansion
    public float lightSpeed;

    private float nextExplodeTime;
    private GameObject player;
    private Light light;
    private SphereCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponentInChildren<Light>();
        player = FrequentlyAccessed.Instance.player;
        collider = GetComponent<SphereCollider>();

        collider.enabled = false;
        nextExplodeTime = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > explodeRadius)
        {
            nextExplodeTime = -1;
        }
        else if (nextExplodeTime < 0)
        {
            nextExplodeTime = Time.time + explodeTime;
        }
        else if (Time.time >= nextExplodeTime)
        {
            StartCoroutine(Explode());
        }

        if (nextExplodeTime > 0)
            light.range += Time.deltaTime * lightSpeed;
        else if (light.range > 0)
            light.range -= Time.deltaTime * lightSpeed;
    }

    private IEnumerator Explode()
    {
        Debug.Log("esplodo");

        collider.radius = explodeRadius;
        collider.enabled = true;

        yield return new WaitForSeconds(0.1f);

        GetComponent<EnemyHealthManager>().TakeDamage(200);

        yield return null;
    }
}
