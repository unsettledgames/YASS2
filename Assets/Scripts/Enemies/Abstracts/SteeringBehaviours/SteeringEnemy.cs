using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteeringBehaviour
{
    Static, Follow
}

public class CollisionObject
{
    public GameObject gameObject;
    public float radius;

    public CollisionObject(GameObject go, float r)
    {
        this.gameObject = go;
        this.radius = r;
    }
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

    protected Vector3 currentCollisionForce;
    protected List<CollisionObject> objectsToAvoid;
    protected float collisionRadius;
    protected float noCollisionLerp;

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

        noCollisionLerp = 0;
        currentCollisionForce = Vector3.zero;
        objectsToAvoid = new List<CollisionObject>();

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
        List<CollisionObject> copyList = new List<CollisionObject>(objectsToAvoid);
        Vector3 ret = Vector3.zero;

        if (hitSomething)
        {
            // Creating an object containing info about the thing to avoid
            CollisionObject toAdd = new CollisionObject(
                    hit.collider.gameObject,
                    Vector3.Distance(hit.collider.gameObject.transform.position, hit.point
                ));
            // Adding it to the list if I haven't taken it into consideration yet
            if (!objectsToAvoid.Contains(toAdd))
                objectsToAvoid.Add(toAdd);
        }

        // Adding forces for each object to avoid
        for (int i=0; i<copyList.Count; i++)
        {
            GameObject currObject = copyList[i].gameObject;
            float currRadius = copyList[i].radius;

            currentCollisionForce = transform.position + transform.forward * collisionCheckDistance - currObject.transform.position;
            // Normalize it and scale it by the force magnitude
            currentCollisionForce = currentCollisionForce.normalized * collisionAvoidanceMagnitude *
            // Increase the force depending on how near the enemy is to the object to avoid
            ((collisionCheckDistance / 2) / (Mathf.Abs(currRadius -
            (Vector3.Distance(transform.position, currObject.transform.position)))));

            ret += currentCollisionForce;

            // If the contribute of the collision vector is smol, I've avoided the obstacle and I can remove it from the list
            if (Vector3.Distance(currentCollisionForce, Vector3.zero) < 0.5f)
            {
                objectsToAvoid.Remove(copyList[i]);
            }
        }

        return currentCollisionForce;
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
