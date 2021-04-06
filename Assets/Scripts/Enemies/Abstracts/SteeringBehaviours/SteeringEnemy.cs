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
        Vector3 desiredVelocity = currentTarget.transform.position - transform.position;
        Vector3 finalVelocity;

        if (allowFollow)
        {
            if (currentTarget != null)
            {
                finalVelocity = desiredVelocity - steeringPhysics.velocity;

                AddForce(finalVelocity.normalized * followForceMagnitude);
            }

            if (rotate)
                transform.LookAt(currentTarget.transform);

            ClampVelocity(followMaxSpeed);
        }

        if (allowEscape)
        {
            if (currentTarget != null)
            {
                desiredVelocity *= -1;
                finalVelocity = desiredVelocity - steeringPhysics.velocity;

                AddForce(finalVelocity.normalized * escapeForceMagnitude);
            }

            if (rotate)
                transform.LookAt(desiredVelocity);

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
