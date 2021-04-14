﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviours
{
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
}

namespace SteeringBehaviours
{

    public class SteeringEnemy : OptimizedMonoBehaviour
    {
        public GameObject[] pathObjects;
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

        // Components
        protected Rigidbody steeringPhysics;
        private Rigidbody targetPhysics;

        // Behaviour stuff
        protected Vector3 currentTarget;
        protected bool steeringOverriden;
        protected Vector3 startVelocity;
        protected Vector3 startPosition;
        protected Vector3 currentVelocity;
        protected Vector3 currentWanderDisplacement;
        private Vector3 rotationAngle;
        private float wanderAngleChange;

        // Collisions stuff
        protected Vector3 currentCollisionForce;
        protected List<CollisionObject> objectsToAvoid;
        protected float collisionRadius;
        protected float noCollisionLerp;

        // Path stuff
        protected Path currentPath;
        protected GameObject tempTargetObject;
        protected bool isFollowingPath;

        private float startFollowMagnitude;

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
            //wanderAngleChange = 1f;

            noCollisionLerp = 0;
            currentCollisionForce = Vector3.zero;
            objectsToAvoid = new List<CollisionObject>();

            isFollowingPath = false;
            currentPath = null;
            tempTargetObject = new GameObject();

            startFollowMagnitude = followForceMagnitude;

            // Applying start velocity if the behaviour requires it
            if (allowWander)
                steeringPhysics.velocity = startVelocity * wanderMaxSpeed;

            Test();
        }

        private void Test()
        {
            List<PathNode> pathList = new List<PathNode>();
            Path path;

            for (int i=0; i<pathObjects.Length; i++)
            {
                pathList.Add(new PathNode(pathObjects[i].transform.position, 8f));
            }

            path = new Path(pathList, false, true);

            StartPath(path);
        }

        protected void Update()
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            followForceMagnitude = startFollowMagnitude;

            // If I have to follow a path, I do so
            if (isFollowingPath)
            {
                // Setting the current node as the target
                SetTarget(tempTargetObject);

                // If I'm near to the current target, I choose the next target
                if (Vector3.Distance(transform.position, currentPath.GetCurrentNode().position) < currentPath.GetCurrentNode().radius)
                {
                    if (currentPath.Next() != null)
                        tempTargetObject.transform.position = currentPath.GetCurrentNode().position;
                    else
                        isFollowingPath = false;
                }

                Debug.Log("Relative velocity: " + transform.InverseTransformPoint(currentVelocity) + " is first or last: " + currentPath.Reversed());

                if (transform.InverseTransformPoint(currentVelocity).x > 0 && currentPath.Reversed())
                    followForceMagnitude = startFollowMagnitude * 1.5f;

                currentVelocity = Seek(tempTargetObject.transform.position, followMaxSpeed, followForceMagnitude,
                    Vector3.zero, 0);

                Debug.DrawLine(transform.position, tempTargetObject.transform.position);
            }
            // Otherwise I handle the different behaviours
            else
            {
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
            }

            // Handling collisions
            if (avoidCollisions)
            {
                currentVelocity += AvoidCollisions();
            }

            // Applying the velocity via Euler integration
            transform.position += currentVelocity * Time.deltaTime;

            // Rotating the object
            if (rotate)
                transform.LookAt(transform.position + currentVelocity);
        }

        /**
         * Computes the force necessary to push the object away from the nearest obstacle
         */ 
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
            for (int i = 0; i < copyList.Count; i++)
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

        /**
         * Computes the velocity necessary to make the object randomly wander around
         */ 
        private Vector3 Wander()
        {
            Vector3 ret;
            Vector3 sphereCenter = transform.position + currentVelocity.normalized * sphereDistance;

            currentWanderDisplacement = Quaternion.Euler(rotationAngle) * currentWanderDisplacement;
            rotationAngle += (Vector3.one).normalized * wanderAngleChange;

            ret = (sphereCenter + currentWanderDisplacement * sphereRadius - transform.position).normalized * wanderMaxSpeed;

            return ret;
        }

        /** 
         * Seeks "target" with speed "speed", smoothing the movement depending on "steeringForce", using "targetVelocity" to
         * forecast the position of the target with precision "foreCastPrecision"
         */ 
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

        /**
         * Escapes from a target, the parameters have the same meaning of the Seek behaviour, but the object escapes from
         * target instead of seeking it
         */ 
        private Vector3 Escape(Vector3 target, float speed, float steeringForce, Vector3 targetVelocity)
        {
            return -Seek(target, speed, steeringForce, targetVelocity, escapeForecastPrecision);
        }

        /** 
         * Starts the path "path" and makes the object follow it using the Seek behaviour
         */ 
        public void StartPath(Path path)
        {
            this.currentPath = path;
            this.isFollowingPath = true;

            tempTargetObject.transform.position = path.GetCurrentNode().position;
        }

        /**
         * Reverses the path and makes the object turn to where it started
         */ 
        public void ReversePath()
        {
            this.currentPath.ReverseNow();
        }

        /**
         * Pauses the current path so that the object can keep following it later
         */ 
        public void PausePath()
        {
            if (this.currentPath != null)
                this.isFollowingPath = false;
        }

        /**
         * Resumes the following of the last saved path, if there is one
         */ 
        public void ResumePath()
        { 
            if (this.currentPath != null)
                this.isFollowingPath = true;
        }
        public void EndPath()
        {
            this.currentPath = null;
            this.isFollowingPath = false;
        }

        /**
         * Sets toSet as a target
         */ 
        public void SetTarget(GameObject toSet)
        {
            target = toSet;
            currentTarget = toSet.transform.position;
            targetPhysics = toSet.GetComponent<Rigidbody>();
        }

        /**
         * Sets "toSet" as the object velocity 
         */ 
        public void SetVelocity(Vector3 toSet)
        {
            steeringPhysics.velocity = toSet;
        }
        /**
         * Adds the force "toAdd" to the rigidbody of this object
         */ 
        public void AddForce(Vector3 toAdd)
        {
            steeringPhysics.AddForce(toAdd, ForceMode.Force);
        }

        /**
         * Functions used to override the steering behaviours: with SetVelocity and AddForce it's possible to manually contorl
         * the object
         */ 
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
}