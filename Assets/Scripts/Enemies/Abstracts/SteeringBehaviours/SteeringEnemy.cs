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
    [HideInInspector] public float wanderForceMagnitude;

    protected Rigidbody steeringPhysics;
    protected Vector3 currentTarget;

    protected bool steeringOverriden;
    protected Vector3 startVelocity;
    protected Vector3 startPosition;
    protected Vector3 currentVelocity;
    protected Vector3 currentWanderDisplacement;

    private float nextWanderChangeDirectionTime;
    private Vector3 rotationAngle;
    private float wanderAngleChange;

    // Start is called before the first frame update
    protected void Start()
    {
        steeringOverriden = false;
        steeringPhysics = GetComponent<Rigidbody>();

        startVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        startPosition = transform.position;

        currentTarget = target.transform.position;

        currentWanderDisplacement = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized).normalized * wanderMaxSpeed;
        rotationAngle = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * wanderAngleChange;
        wanderAngleChange = 1f;

        // Applying start velocity if the behaviour requires it
        if (allowWander)
        {
            steeringPhysics.velocity = startVelocity * wanderMaxSpeed;
        }
    }

    protected void Update()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // FOLLOW behaviour
        if (allowFollow)
        {
            currentVelocity = Seek(target.transform.position, followMaxSpeed, followForceMagnitude);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, followDistance / distance);

            transform.position += currentVelocity * Time.deltaTime;
        }

        // ESCAPE behaviour
        if (allowEscape)
        {
            currentVelocity = Escape(target.transform.position, escapeMaxSpeed, escapeForceMagnitude);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, distance / escapeMinDistance);

            transform.position += currentVelocity * Time.deltaTime;
        }

        // WANDER behaviour
        if (allowWander)
        {
            currentVelocity = Wander() * Time.deltaTime;
            transform.position += currentVelocity;

            Debug.Log("currentVelocity: " + currentVelocity.x + ", " + currentVelocity.y + ", " + currentVelocity.z);
        }
        
        if (rotate)
            transform.LookAt(transform.position + currentVelocity);
        
    }

    private Vector3 Wander()
    {
        Vector3 ret;
        Vector3 sphereCenter = transform.position + currentVelocity.normalized * sphereDistance;

        currentWanderDisplacement = Quaternion.Euler(rotationAngle) * currentWanderDisplacement;
        rotationAngle += (Vector3.one).normalized * wanderAngleChange;

        ret = (sphereCenter + currentWanderDisplacement).normalized * wanderMaxSpeed;

        return ret;
    }

    private Vector3 Seek(Vector3 target, float speed, float steeringForce)
    {
        Vector3 ret;
        Vector3 steering;

        // Desired velocity (simply target - currentPosition)
        Vector3 desired = (target - transform.position).normalized * speed;

        // Computing steering vector, the difference between the desired and the current velocity.
        // Clamping by followForceMagnitude to decide how fast the object changes direction
        steering = (desired - currentVelocity).normalized * steeringForce;
        // Adding steering force to the final vector
        ret = (currentVelocity + steering).normalized * speed;

        return ret;
    }

    private Vector3 Escape(Vector3 target, float speed, float steeringForce)
    {
        return -Seek(target, speed, steeringForce);
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
