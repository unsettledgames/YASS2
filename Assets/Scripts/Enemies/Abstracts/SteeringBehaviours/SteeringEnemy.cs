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

    // Follow behaviour attributes
    [HideInInspector] public float followForceMagnitude;
    [HideInInspector] public float followMaxSpeed;
    [HideInInspector] public float followDistance;

    // Escape behaviour attributes
    [HideInInspector] public float escapeForceMagnitude;
    [HideInInspector] public float escapeMaxSpeed;
    [HideInInspector] public float escapeMinDistance;

    protected Rigidbody steeringPhysics;
    protected bool steeringOverriden;
    protected GameObject currentTarget;

    // Start is called before the first frame update
    protected void Start()
    {
        steeringOverriden = false;
        steeringPhysics = GetComponent<Rigidbody>();

        currentTarget = target;
    }

    protected void Update()
    {
        Vector3 desiredVelocity = (currentTarget.transform.position - transform.position).normalized;
        float distanceFromTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        Vector3 finalVelocity;

        if (allowFollow)
        {
            if (currentTarget != null)
            {
                // Slow down when you're too near to the target
                if (distanceFromTarget > followDistance)
                    finalVelocity = Vector3.Lerp(steeringPhysics.velocity, desiredVelocity * followMaxSpeed, 0.5f);
                else
                    finalVelocity = Vector3.Lerp(steeringPhysics.velocity, Vector3.zero, 0.01f);

                steeringPhysics.velocity = finalVelocity;
            }

            if (rotate)
                transform.LookAt(currentTarget.transform);

            ClampVelocity(followMaxSpeed);
        }

        if (allowEscape)
        {
            if (currentTarget != null)
            {
                // The desired velocity points away from the target
                desiredVelocity *= -1;
                // Slow down when you're too far from the target
                if (distanceFromTarget < escapeMinDistance)
                    finalVelocity = Vector3.Lerp(steeringPhysics.velocity, desiredVelocity * escapeMaxSpeed, 0.5f);
                else
                    finalVelocity = Vector3.Lerp(steeringPhysics.velocity, Vector3.zero, 0.1f);

                steeringPhysics.velocity = finalVelocity;
            }

            if (rotate)
                transform.LookAt(transform.position + desiredVelocity);

            ClampVelocity(escapeMaxSpeed);
        }        
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
