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
    

    [Header("Projectiles")]
    public GameObject standardBullet;
    public GameObject missile;

    [Header("Technical")]
    public float timeBetweenDodge;

    // Components
    private Rigidbody physics;

    // State
    private bool isDodging = false;
    private bool externallyControlled = false;

    private Vector3 currentVelocity;
    private Quaternion currentRotation;

    private Vector3 prevVelocity;
    private Vector3 prevLookAt;
    

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();

        prevLookAt = transform.position + Vector3.forward * InputManager.Instance.mouseZPosition;
        prevVelocity = Vector3.zero;

        currentRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!externallyControlled)
        {
            VelocityManagement();
            TorqueManagement();
            PointTowardsMouse();

            ShootManagement();
        }

        prevVelocity = physics.velocity;
    }

    private void VelocityManagement()
    {
        SetVelocity(transform.forward * standardSpeedMagnitude);
    }

    private void TorqueManagement()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float x = InputManager.Instance.mousePosition.x;

        //transform.RotateAround(transform.position, Vector3.forward, Mathf.Lerp(0, 60, Mathf.Abs(x) * -Mathf.Sign(x)));
    }

    private void PointTowardsMouse()
    {
        Vector3 mousePosition = InputManager.Instance.mousePosition;

        Debug.Log(mousePosition.x);

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

        prevLookAt = mousePosition - transform.position;
    }

    private void ShootManagement()
    {

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
