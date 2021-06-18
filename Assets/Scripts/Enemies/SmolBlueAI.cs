using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteeringBehaviours;

public class SmolBlueAI : MonoBehaviour
{
    [Header("Behaviour")]
    public float triggerDistance;
    public Vector2 speedVariation;

    [Header("Shooting")]
    public float shotRate;
    public float flurryRate;
    public float flurryDuration;
    public float flurryNoise = 0.5f;
    public float aimTween;

    public GameObject[] shotSpawns;
    public GameObject projectile;

    // State
    private bool triggered = false;
    private SteeringBehaviour currentBehaviour;
    // Shoot state
    private float nextFlurryTime;
    private float nextFlurryEndTime;
    private float nextShootTime;
    private bool setTimings = false;
    
    private PlayerShipController player;
    private Rigidbody playerPhysics;
    private SteeringEnemy behaviour;

    // Start is called before the first frame update
    void Start()
    {
        player = FrequentlyAccessed.Instance.playerController;
        behaviour = GetComponent<SteeringEnemy>();
        playerPhysics = player.GetComponent<Rigidbody>();

        behaviour.towardsTarget = false;
        behaviour.avoidCollisions = true;

        currentBehaviour = SteeringBehaviour.Wander;
        behaviour.SetBehaviour(currentBehaviour);
        behaviour.followMaxSpeed = speedVariation.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > triggerDistance && !triggered)
            behaviour.SetBehaviour(SteeringBehaviour.Wander);
        else
        {
            // Triggering the enemy
            behaviour.towardsTarget = true;
            triggered = true;
            behaviour.followMaxSpeed = speedVariation.y;
            behaviour.avoidCollisions = false;

            // Reset the timings if I'm being triggered
            if (currentBehaviour == SteeringBehaviour.Wander)
            {
                UpdateAllTimings();
                setTimings = true;

                currentBehaviour = SteeringBehaviour.Seek;
            }

            // Seeking
            behaviour.SetBehaviour(SteeringBehaviour.Seek);
            // Shooting
            ShootManagement();

            // Oscillate between speeds
            // TODO
        }
    }

    private void ShootManagement()
    {
        // If I'm in a flurry, I shoot
        if (Time.time >= nextFlurryTime && Time.time <= nextFlurryEndTime)
        {
            setTimings = false;

            if (Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + shotRate;
                // Getting player velocity to anticipate them
                Vector3 playerVel = playerPhysics.velocity;
                // Instantiate projectiles
                GameObject[] projectiles = new GameObject[shotSpawns.Length];

                for (int i = 0; i < projectiles.Length; i++)
                {
                    projectiles[i] = Instantiate(projectile, shotSpawns[i].transform.position,
                        Quaternion.Euler(Vector3.zero));
                    projectiles[i].transform.LookAt(transform.forward + playerVel.normalized * aimTween);
                }
            }
        }
        // If I haven't set the times for the next flurry, I do so, else I just do nothing and wait
        else if (!setTimings)
        {
            UpdateAllTimings();
            setTimings = true;
        }
    }

    private void UpdateAllTimings()
    {
        nextFlurryTime = Time.time + flurryRate + Random.Range(-flurryNoise, flurryNoise);
        nextFlurryEndTime = Time.time + flurryRate + flurryDuration + Random.Range(-flurryNoise, flurryNoise);
        nextShootTime = Time.time + flurryRate;
    }
}
