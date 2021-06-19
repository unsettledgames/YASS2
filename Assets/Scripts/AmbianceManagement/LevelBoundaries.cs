using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundaries : MonoBehaviour
{
    [Header("Fresnel")]
    public float minFresnel; 
    public float maxFresnel;

    [Header("Distances")]
    public float fresnelDistance;
    public float slowDownDistance;
    public float stopDistance;
    public float radius;

    [Header("U turn data")]
    public float turnDuration = 2f;
    public float turnSpeed = 1f;
    public float turnVelocityDuration = 1f;

    private PlayerShipController player;
    private Material fresnelMaterial;
    private Rigidbody playerPhysics;
    private float maxPlayerSpeed;

    // State
    private bool turning = false;
    private float turnEndTime = -1;
    private float velocityEndTime = -1;
    // Start is called before the first frame update
    void Start()
    {
        player = FrequentlyAccessed.Instance.playerController;
        playerPhysics = player.GetComponent<Rigidbody>();
        fresnelMaterial = GetComponent<MeshRenderer>().sharedMaterial;

        maxPlayerSpeed = player.standardSpeedMagnitude;
        transform.localScale = Vector3.one * radius;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        float dotProduct;
        float t;
        Vector3 normal;

        if (distance > fresnelDistance)
        {
            // Fresnel intensity depends on player distance from the boundaries
            t = (distance - fresnelDistance) / (stopDistance - fresnelDistance);
            // And it depends on the dot product between player.forward and the normal
            RaycastHit hit;
            bool hitSphere = Physics.Raycast(player.transform.position, player.transform.forward, out hit,
                radius, LayerMask.GetMask("Boundaries"));

            if (hitSphere)
            {
                // Getting the normal and the dot product
                normal = hit.normal;
                dotProduct = Vector3.Dot(normal, player.transform.forward);

                // The more the dot equals -mag(player.forward) * mag(normal), the less visible the fresnel is
                t *= Mathf.Lerp(1, 0, dotProduct / (-player.transform.forward.magnitude * normal.magnitude));
            }

            // Start showing fresnel
            fresnelMaterial.SetFloat("_FresnelPower", Mathf.Lerp(minFresnel, maxFresnel, t));
        }

        // Slow down the ship depending on the distance and the normal (or just make an U turn after a while?)
        if (distance > slowDownDistance && !turning)
        {
            t = 1 - (distance - slowDownDistance) / (stopDistance - slowDownDistance);
            playerPhysics.velocity *= t * Mathf.Sign(t);

            // If the player is too near, u turn
            if (t < 0.1)
            {
                turning = true;
                turnEndTime = Time.time + turnDuration;
                player.TakeControl();
            }
        }
        else if (Time.time <= turnEndTime)
        {
            Debug.DrawRay(player.transform.position + player.transform.forward * 2, player.transform.up, Color.blue);
            Vector3 dir = Vector3.zero;

            dir.x = 0;
            dir.y = 1;
            dir.z = 0;

            player.SetExternalVelocity(player.transform.forward.normalized * player.standardSpeedMagnitude);
            player.transform.rotation *= Quaternion.Euler(Vector3.right * Time.deltaTime * turnSpeed);
        }
        else if (turning && (Time.time - turnEndTime) < 0.1f && Time.time > velocityEndTime)
        {
            velocityEndTime = Time.time + turnVelocityDuration;
        }
        else if (turning && Time.time >= velocityEndTime)
        {
            turning = false;
            Debug.Log("END OF U TURN");
            player.ReleaseControl();
        }
    }

    private IEnumerator TurnShip()
    {
        Debug.Log("Started");
        turning = true;
        player.TakeControl();

        // Changing the velocity
        StartCoroutine(MovementUtility.SlideVector3(playerPhysics.velocity, -playerPhysics.velocity, turnSpeed,
            Consts.easeCurve, UpdatePlayerVelocity));

        yield return new WaitForSeconds(turnDuration);

        player.ReleaseControl();
        turning = false;
    }

    private void UpdatePlayerVelocity(Vector3 vel)
    {
        playerPhysics.velocity = vel;
    }
}
