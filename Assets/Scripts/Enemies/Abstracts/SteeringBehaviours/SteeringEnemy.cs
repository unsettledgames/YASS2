using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteeringBehaviour
{
    Static, Follow
}

public class SteeringEnemy : OptimizedMonoBehaviour
{
    [Header("General properties")]
    public GameObject target;
    public bool rotate;

    [Header("Behaviours")]
    public bool allowStatic;
    public bool allowFollow;
    public bool allowEscape;
    public bool allowWander;

    // Follow behaviour attributes
    [HideInInspector] public float followForceMagnitude;
    [HideInInspector] public float followMaxSpeed;
    [HideInInspector] public float followDistance;

    // Escape behaviour attributes
    [HideInInspector] public float escapeForceMagnitude;
    [HideInInspector] public float escapeMaxSpeed;
    [HideInInspector] public float escapeMinDistance;

    // Wander behaviour attributes
    [HideInInspector] public float sphereDistance;
    [HideInInspector] public float sphereRadius;
    [HideInInspector] public float wanderMaxSpeed;
    [HideInInspector] public float wanderChangeDirectionRate;

    protected Rigidbody steeringPhysics;
    protected Vector3 currentTarget;

    protected bool steeringOverriden;
    protected Vector3 startVelocity;
    protected Vector3 startPosition;
    protected Vector3 currentVelocity;

    private float nextWanderChangeDirectionTime;

    // Start is called before the first frame update
    protected void Start()
    {
        steeringOverriden = false;
        steeringPhysics = GetComponent<Rigidbody>();

        startVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        startPosition = transform.position;

        currentTarget = target.transform.position;

        nextWanderChangeDirectionTime = Time.time + wanderChangeDirectionRate;

        // Applying start velocity if the behaviour requires it
        if (allowWander)
        {
            steeringPhysics.velocity = startVelocity * wanderMaxSpeed;
        }
    }

    protected void Update()
    {
        /*
        // Saving general attributes that will be used in Update
        Vector3 desiredVelocity = (currentTarget - transform.position).normalized;
        Vector3 currentVelocity = steeringPhysics.velocity;
        Vector3 finalVelocity;

        float distanceFromTarget = Vector3.Distance(transform.position, currentTarget);
       

        // FOLLOW BEHAVIOUR
        if (allowFollow)
        {
            currentTarget = target.transform.position;

            if (currentTarget != null)
            {
                Seek(currentTarget, currentVelocity, desiredVelocity, followMaxSpeed, followDistance);
            }

            if (rotate)
                transform.LookAt(currentTarget);

            ClampVelocity(followMaxSpeed);
        }

        // ESCAPE BEHAVIOUR
        if (allowEscape)
        {
            currentTarget = target.transform.position;

            if (currentTarget != null)
            {
                // The desired velocity points away from the target
                desiredVelocity *= -1;
                // Slow down when you're too far from the target
                if (distanceFromTarget < escapeMinDistance)
                    finalVelocity = Vector3.Lerp(currentVelocity, desiredVelocity * escapeMaxSpeed, 0.5f);
                else
                    finalVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 0.1f);

                steeringPhysics.velocity = finalVelocity;
            }

            if (rotate)
                transform.LookAt(transform.position + desiredVelocity);

            ClampVelocity(escapeMaxSpeed);
        }        

        // WONDER BEHAVIOUR
        if (allowWander)
        {
            if (Vector3.Distance(currentTarget, transform.position) < 1)
            {
                currentTarget = startPosition + new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
            }

            Debug.Log("current target: " + currentTarget);

            Seek(currentTarget, currentVelocity, desiredVelocity, wanderMaxSpeed, 0);
            /*
            Vector3 displacementDirection = ObjectPooler.Instance.GetVector3();
            // Placing the sphere
            Vector3 sphereCenter = transform.position + currentVelocity.normalized * sphereDistance;
            
            // Computing the displacement direction if enough time has passed
            if (Time.time >= nextWanderChangeDirectionTime)
            {
                Debug.Log(Time.time);
                displacementDirection.x = Random.Range(-1f, 1f);
                displacementDirection.y = Random.Range(-1f, 1f);
                displacementDirection.z = Random.Range(-1f, 1f);

                // Reset the direction change timer
                nextWanderChangeDirectionTime = Time.time + wanderChangeDirectionRate;
            }

            // Computing the displacement point
            Vector3 displacement = sphereCenter + displacementDirection;
            // Computing the desired velocity
            desiredVelocity = (displacement - transform.position).normalized * wanderMaxSpeed;
            
            // Lerping towards the desired velocity
            steeringPhysics.AddForce((desiredVelocity - currentVelocity).normalized * wanderMaxSpeed);

            if (rotate)
                transform.LookAt(transform.position + steeringPhysics.velocity);

            ObjectPooler.Instance.EnqueueVector3(displacementDirection);
            ClampVelocity(wanderMaxSpeed);
            
            if (rotate)
                transform.LookAt(transform.position + steeringPhysics.velocity);
        }
        */

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (allowFollow)
        {
            currentVelocity = Seek(target.transform.position);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, followDistance / distance);

            transform.position += currentVelocity * Time.deltaTime;
        }

        if (allowEscape)
        {
            currentVelocity = Escape(target.transform.position);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, distance / escapeMinDistance);

            transform.position += currentVelocity * Time.deltaTime;
        }

        if (allowWander)
        {

        }

        if (rotate)
            transform.LookAt(transform.position + currentVelocity);
        
    }

    private Vector3 Seek(Vector3 target, float minDistance = 0)
    {
        Vector3 ret;
        Vector3 steering;

        // Distance from the player
        float distance = Vector3.Distance(transform.position, target);

        // Desired velocity (simply target - currentPosition)
        Vector3 desired = Vector3.ClampMagnitude(target - transform.position, followMaxSpeed);

        // Computing steering vector, the difference between the desired and the current velocity.
        // Clamping by followForceMagnitude to decide how fast the object changes direction
        steering = Vector3.ClampMagnitude(desired - currentVelocity, followForceMagnitude);
        // Adding steering force to the final vector
        ret = Vector3.ClampMagnitude(currentVelocity + steering, followMaxSpeed);

        return ret;
    }

    private Vector3 Escape(Vector3 target, float maxDistance = Mathf.Infinity)
    {
        return -Seek(target);
    }

    /*
    private void Seek(Vector3 target, Vector3 currentVelocity, Vector3 desiredVelocity, float maxSpeed, float distanceFromTarget = 0)
    {
        Vector3 finalVelocity;

        // Slow down when you're too near to the target
        if (Vector3.Distance(transform.position, target) > distanceFromTarget)
            finalVelocity = Vector3.Lerp(currentVelocity, desiredVelocity * maxSpeed, 0.5f);
        else
            finalVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 0.1f);

        steeringPhysics.velocity = finalVelocity;
    }*/

    private void ClampVelocity(float maxSpeed)
    {
        if (!steeringOverriden)
        {
            steeringPhysics.velocity = Vector3.ClampMagnitude(steeringPhysics.velocity, maxSpeed);
        }
    }

    public void SetTarget(GameObject toSet)
    {
        currentTarget = toSet.transform.position;
    }
    public void SetTarget(Vector3 toSet)
    {
        currentTarget = toSet;
    }

    public void SetVelocity(Vector3 toSet)
    {
        steeringPhysics.velocity = toSet;
    }

    public void AddForce(Vector3 toAdd)
    {
        steeringPhysics.AddForce(toAdd, ForceMode.Force);
    }
    
    public void OverrideSteering()
    {
        steeringOverriden = true;
    }
    public void OverrideSteering(float time)
    {
        StartCoroutine(OverrideRoutine(time));
    }
    private IEnumerator OverrideRoutine(float time)
    {
        OverrideSteering();
        yield return new WaitForSeconds(time);
        SetSteering();
    }

    public void SetSteering()
    {
        steeringOverriden = false;
    }


}
