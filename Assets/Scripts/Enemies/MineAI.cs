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

    private float nextExplodeTime;
    private GameObject player;
    private SphereCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        player = FrequentlyAccessed.Instance.player;
        collider = GetComponent<SphereCollider>();

        collider.enabled = false;
        nextExplodeTime = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        Debug.Log("Distanza: " + distance);

        if (distance > explodeRadius)
        {
            nextExplodeTime = Mathf.Infinity;
        }
        else if (nextExplodeTime == Mathf.Infinity)
        {
            nextExplodeTime = Time.deltaTime + explodeTime;
        }
        else if (Time.time >= nextExplodeTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        collider.radius = explodeRadius;
        collider.enabled = true;

        GetComponent<EnemyHealthManager>().TakeDamage(200);
    }
}
