using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingSteeringEnemy : SteeringEnemy
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (currentTarget != null)
        {
            Vector3 desiredVelocity = currentTarget - transform.position;
            Vector3 finalVelocity = desiredVelocity - steeringPhysics.velocity;

            AddForce(finalVelocity.normalized * followForceMagnitude);
        }
    }
}
