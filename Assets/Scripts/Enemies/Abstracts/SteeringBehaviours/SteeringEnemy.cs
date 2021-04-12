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

    [Header("Collision avoidance")]
    public bool avoidCollisions;
    [HideInInspector] public float collisionCheckDistance;
    [HideInInspector] public string collisionTagsToAvoid;
    [HideInInspector] public float collisionAvoidanceMagnitude;
    [HideInInspector] public LayerMask collisionLayerMask;

    // Static behaviour
    [HideInInspector] public Vector3 staticVelocity;

    // Follow behaviour attributes
    [HideInInspector] public float followForceMagnitude;
    [HideInInspector] public float followMaxSpeed;
    [HideInInspector] public float followDistance;
    [HideInInspector] public bool followForecastTarget;
    [HideInInspector] public float followForecastPrecision;

    // Escape behaviour attributes
    [HideInInspector] public float escapeForceMagnitude;
    [HideInInspector] public float escapeMaxSpeed;
    [HideInInspector] public float escapeMinDistance;
    [HideInInspector] public bool escapeForecastTarget;
    [HideInInspector] public float escapeForecastPrecision;

    // Wander behaviour attributes
    [HideInInspector] public float sphereDistance;
    [HideInInspector] public float sphereRadius;
    [HideInInspector] public float wanderMaxSpeed;
    [HideInInspector] public float wanderForceMagnitude;

    protected Rigidbody steeringPhysics;
    protected Vector3 currentTarget;

    protected bool steeringOverriden;
    protected Vector3 startVelocity;
    protected Vector3 startPosition;
    protected Vector3 currentVelocity;
    protected Vector3 currentWanderDisplacement;

    private Vector3 rotationAngle;
    private float wanderAngleChange;
    private Rigidbody targetPhysics;

    // Start is called before the first frame update
    protected void Start()
    {
        steeringOverriden = false;
        steeringPhysics = GetComponent<Rigidbody>();

        startVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        startPosition = transform.position;

        currentTarget = target.transform.position;
        targetPhysics = target.GetComponent<Rigidbody>();

        currentWanderDisplacement = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized).normalized * wanderMaxSpeed;
        rotationAngle = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * wanderForceMagnitude;

        // Applying start velocity if the behaviour requires it
        if (allowWander)
            steeringPhysics.velocity = startVelocity * wanderMaxSpeed;
    }

    protected void Update()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // STATIC velocity behaviour
        if (allowStatic)
            currentVelocity = staticVelocity;

        // FOLLOW behaviour
        if (allowFollow)
        {
            currentVelocity = Seek(target.transform.position, followMaxSpeed, followForceMagnitude, targetPhysics.velocity, 0);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, followDistance / distance);
        }

        // ESCAPE behaviour
        if (allowEscape)
        {
            currentVelocity = Escape(target.transform.position, escapeMaxSpeed, escapeForceMagnitude, targetPhysics.velocity);
            // Taking distance in account
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, distance / escapeMinDistance);
        }

        // WANDER behaviour
        if (allowWander)
        {
            currentVelocity = Wander();
        }

        // Handling collisions
        if (avoidCollisions)
        {
            currentVelocity += AvoidCollisions();
        }

        transform.position += currentVelocity * Time.deltaTime;

        if (rotate)
            transform.LookAt(transform.position + currentVelocity);
    }

    private Vector3 AvoidCollisions()
    {
        // Firing a raycast
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, transform.forward, out hit, collisionCheckDistance, collisionLayerMask);

        if (hitSomething)
        {
            Vector3 avoidanceForce = hit.normal;

            Debug.DrawLine(transform.position, hit.point, Color.green);
            Debug.DrawLine(transform.position, transform.position + avoidanceForce * 10, Color.red);

            return avoidanceForce.normalized * collisionAvoidanceMagnitude * (collisionCheckDistance / Vector3.Distance(transform.position, hit.point));
        }

        // Non Vector3.zero, ma interpolare tra il precedente vettore di collisioni e Vector3.zero (aumento di Time.deltaTime 
        // il valore usato per lerpare finché non arrivo fino a 1. Dentro if (hitSomething) resetto quel valore (oppure sarebbe
        // interessante sottrarre Time.deltaTime per averlo ancora più smooth ma non so cosa succede onestamente
        return Vector3.zero;
    }

    private Vector3 Wander()
    {
        Vector3 ret;
        Vector3 sphereCenter = transform.position + currentVelocity.normalized * sphereDistance;

        currentWanderDisplacement = Quaternion.Euler(rotationAngle) * currentWanderDisplacement;
        rotationAngle += (Vector3.one).normalized * wanderAngleChange;

        ret = (sphereCenter + currentWanderDisplacement*sphereRadius - transform.position).normalized * wanderMaxSpeed;

        return ret;
    }

    private Vector3 Seek(Vector3 target, float speed, float steeringForce, Vector3 targetVelocity, float foreCastPrecision = 0)
    {
        Vector3 ret;
        Vector3 steering;

        if (followForecastTarget)
            target += foreCastPrecision * targetVelocity * (Vector3.Distance(transform.position, target) / speed);

        // Desired velocity (simply target - currentPosition)
        Vector3 desired = (target - transform.position).normalized * speed;

        // Computing steering vector, the difference between the desired and the current velocity.
        // Clamping by steeringForce to decide how fast the object changes direction
        steering = (desired - currentVelocity).normalized * steeringForce;

        // Adding steering force to the final vector
        ret = (currentVelocity + steering).normalized * speed;

        return ret;
    }

    private Vector3 Escape(Vector3 target, float speed, float steeringForce, Vector3 targetVelocity)
    {
        return -Seek(target, speed, steeringForce, targetVelocity, escapeForecastPrecision);
    }

    private void ClampVelocity(float maxSpeed)
    {
        if (!steeringOverriden)
        {
            steeringPhysics.velocity = Vector3.ClampMagnitude(steeringPhysics.velocity, maxSpeed);
        }
    }

    public void SetTarget(GameObject toSet)
    {
        target = toSet;
        currentTarget = toSet.transform.position;
        targetPhysics = toSet.GetComponent<Rigidbody>();
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
