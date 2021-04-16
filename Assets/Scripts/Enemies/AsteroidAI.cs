using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidAI : SpawnedEnemy
{
    public float colliderActivationDistance = 30f;
    public float maxRotationMagnitude;
    public float maxVelocityMagnitude;

    private Collider collider;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody physics = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        player = FrequentlyAccessed.Instance.player;


        physics.velocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized * maxVelocityMagnitude;

        physics.angularVelocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized * maxRotationMagnitude;

        collider.enabled = false;
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < colliderActivationDistance)
            collider.enabled = true;
    }
}
