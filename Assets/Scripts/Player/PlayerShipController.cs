using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
    [Header("Movement velocities")]
    public float standardSpeedMagnitude;
    public float maxSpeedMagnitude;

    [Header("Components")]
    public GameObject model;
    public GameObject[] standardShootSpawns;
    public GameObject[] missileSpawns;
    public GameObject[] specialSpawn;
    
    [Header("Torque")]
    public float torqueSpeed;
    public float torqueTween;
    public float maxTorque;

    [Header("Tweening")]
    public float turnTween;
    public float accelerationTween;


    [Header("Shooting management")]
    public float shootRate = 0.15f;
    public GameObject standardBullet;
    public GameObject missile;

    [Header("Technical")]
    public float timeBetweenDodge;

    // Components
    private Rigidbody physics;

    // State
    private bool isDodging = false;
    private bool externallyControlled = false;

    // Shooting stuff
    private float nextShootTime;
    

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();

        nextShootTime = Time.time + shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!externallyControlled)
        {
            VelocityManagement();
            PointTowardsMouse();

            ShootManagement();
        }
    }

    private void VelocityManagement()
    {
        SetVelocity(transform.forward * standardSpeedMagnitude);
    }

    private void ShootManagement()
    {
        Vector3 mouseWorldPos = FrequentlyAccessed.Instance.cameraComponent.ScreenToWorldPoint(InputManager.Instance.globalMousePosition);
        RaycastHit aimHit;
        bool hitObject = Physics.Raycast(transform.position, mouseWorldPos - transform.position, out aimHit, Mathf.Infinity, ~(1 << 8));

        Debug.DrawRay(transform.position, mouseWorldPos - transform.position);

        if (hitObject)
        {
            Debug.Log("Hit: " + aimHit.collider.name);
        }

        if (InputManager.Instance.shootState && Time.time >= nextShootTime)
        {
            Instantiate(standardBullet, standardShootSpawns[0].transform.position, Quaternion.Euler(transform.localEulerAngles));
            Instantiate(standardBullet, standardShootSpawns[1].transform.position, Quaternion.Euler(transform.localEulerAngles));

            nextShootTime = Time.time + shootRate;
        }
    }

    private void PointTowardsMouse()
    {
        Vector3 mousePosition = InputManager.Instance.relativeMousePosition;

        // Setting the new rotation 
        transform.rotation *= Quaternion.Euler(
            -mousePosition.y * Time.deltaTime * torqueSpeed, 
            mousePosition.x * Time.deltaTime * torqueSpeed,
            0
        );

        // Setting rotation (only to make it look cooler)
        model.transform.localEulerAngles = new Vector3(
            Mathf.Lerp(0, 30, Mathf.Abs(mousePosition.y)) * -Mathf.Sign(mousePosition.y), 0, 
            Mathf.Lerp(0, 60, Mathf.Abs(mousePosition.x)) * -Mathf.Sign(mousePosition.x)
        );
    }

    public void SetVelocity(Vector3 toSet)
    {
        if (toSet.magnitude > maxSpeedMagnitude)
            toSet = toSet.normalized * maxSpeedMagnitude;

        physics.velocity = toSet;
    }

    public void TakeControl()
    {
        externallyControlled = true;
    }

    public void ReleaseControl()
    {
        externallyControlled = false;
    }
}
