using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidAI : SpawnedEnemy
{
    public float maxRotationMagnitude;
    public float maxVelocityMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody physics = GetComponent<Rigidbody>();


        physics.velocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized * maxVelocityMagnitude;

        physics.angularVelocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized * maxRotationMagnitude;
    }
}
