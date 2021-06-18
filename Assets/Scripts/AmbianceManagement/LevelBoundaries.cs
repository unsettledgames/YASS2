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

    private PlayerShipController player;
    private Material fresnelMaterial;
    private Rigidbody playerPhysics;
    private float maxPlayerSpeed;
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
        float dotProduct = 0;
        Vector3 normal = Vector3.zero;

        if (distance > fresnelDistance)
        {
            // Fresnel intensity depends on player distance from the boundaries
            float t = (distance - fresnelDistance) / (stopDistance - fresnelDistance);
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

            // Slow down the ship depending on the distance and the normal (or just make an U turn after a while?)
            if (distance > slowDownDistance)
            {
                t = 1 - (distance - slowDownDistance) / (stopDistance - slowDownDistance);

                Debug.Log("T: " + t);

                // The more the player isn't facing the sphere, the more they can move faster
                if (dotProduct != 0 && !normal.Equals(Vector3.zero))
                    t *= Mathf.Lerp(0, 1, dotProduct / (-player.transform.forward.magnitude * normal.magnitude));
                
                playerPhysics.velocity *= t;
            }
        }
    }
}
