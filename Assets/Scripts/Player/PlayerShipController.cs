using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
    [Header("Movement")]
    public float standardSpeedMagnitude;
    public float maxSpeedMagnitude;
    public float minSpeedMagnitude = 3f;
    [Header("Dodge")]
    public float dodgeSpeed = 20f;
    public float dodgeDistance = 5f;
    public float forwardDodgeAmount = 3f;
    public GameObject leftDodgeDirection;
    public GameObject rightDodgeDirection;

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
    public float autoAimDistance = 50f;
    public float autoAimTween = 0.5f;

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
    private GameObject currentTarget;
    

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();

        nextShootTime = Time.time + shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        // This is because when dodging, the ship is controlled externally
        if (!externallyControlled)
        {
            DodgeManagement();
        }

        if (!externallyControlled)
        {
            
            VelocityManagement();
            PointTowardsMouse();

            ShootManagement();
        }
    }

    private void DodgeManagement()
    {
        bool leftDodge = InputManager.Instance.leftDodge;
        bool rightDodge = InputManager.Instance.rightDodge;

        Vector3 dodgeDestination;
        Vector3 angleDestination = model.transform.localEulerAngles;

        if (leftDodge || rightDodge)
        {
            angleDestination.z += leftDodge ? 720 : -720;
            dodgeDestination = (leftDodge ? leftDodgeDirection.transform.position : rightDodgeDirection.transform.position);

            TakeControl();

            // Starting rotation routine
            StartCoroutine(MovementUtility.SlideVector3(
                model.transform.localEulerAngles, angleDestination, dodgeSpeed, Consts.easeCurve,
                UpdateModelRotation, null, null));
            
            // Starting position routine
            StartCoroutine(MovementUtility.SlideVector3(
                transform.position, dodgeDestination, dodgeSpeed, Consts.easeCurve,
                UpdatePosition, ReleaseControl, null));
        }
    }

    private void VelocityManagement()
    {
        // Standard velocity
        Vector3 toSet = transform.forward * standardSpeedMagnitude;

        if (InputManager.Instance.accelerationAmount > 0)
        {
            // Adding acceleration
            toSet = Vector3.Lerp(toSet, toSet.normalized * maxSpeedMagnitude, InputManager.Instance.accelerationAmount);
        }
        else if (InputManager.Instance.accelerationAmount < 0)
        {
            // Removing acceleration
            toSet = Vector3.Lerp(toSet.normalized * minSpeedMagnitude, toSet, 1 + InputManager.Instance.accelerationAmount);
        }

        SetVelocity(toSet);
    }

    private void ShootManagement()
    {
        Vector3 mouseWorldPos = FrequentlyAccessed.Instance.cameraComponent.ScreenToWorldPoint(InputManager.Instance.globalMousePosition);
        RaycastHit aimHit;
        bool hitObject = Physics.Raycast(mouseWorldPos, transform.forward, out aimHit, Mathf.Infinity, ~(1 << 8));

        if (hitObject)
        {
            currentTarget = aimHit.collider.gameObject;
        }
        else
        {
            currentTarget = null;
        }

        if (InputManager.Instance.shootState && Time.time >= nextShootTime)
        {
            // Instantiating projectiles
            GameObject instantiated = Instantiate(standardBullet, standardShootSpawns[0].transform.position, Quaternion.Euler(transform.localEulerAngles));
            GameObject instantiated2 = Instantiate(standardBullet, standardShootSpawns[1].transform.position, Quaternion.Euler(transform.localEulerAngles));

            // Autoaim if the player is hovering an enemy
            if (currentTarget != null && currentTarget.tag.Contains("Enemy"))
            {
                Vector3 toLookAt = currentTarget.transform.position;

                instantiated.transform.LookAt(toLookAt);
                instantiated2.transform.LookAt(toLookAt);
            }

            nextShootTime = Time.time + shootRate;
        }
    }

    private void PointTowardsMouse()
    {
        Vector3 mousePosition = InputManager.Instance.relativeMousePosition;
        Vector3 rotation = ObjectPooler.Instance.GetVector3();
        Vector3 localVelocity = physics.velocity;

        // Setting the new rotation 
        transform.rotation *= Quaternion.Euler(
            -mousePosition.y * Time.deltaTime * torqueSpeed,
            mousePosition.x * Time.deltaTime * torqueSpeed,
            Mathf.Lerp(-maxTorque, maxTorque, 1 - (InputManager.Instance.torqueAmount + 1) / 2)
        );

        Debug.Log("Relative velocity: " + localVelocity / standardSpeedMagnitude);

        rotation.x = Mathf.Lerp(0, 30, Mathf.Abs(mousePosition.y)) * -Mathf.Sign(mousePosition.y);
        rotation.y = 0;
        rotation.z = Mathf.Lerp(0, 60, Mathf.Abs(mousePosition.x)) * -Mathf.Sign(mousePosition.x);
        // Setting rotation (only to make it look cooler)
        model.transform.localEulerAngles = rotation;

        ObjectPooler.Instance.EnqueueVector3(rotation);
    }

    public void SetVelocity(Vector3 toSet)
    {
        if (toSet.magnitude > maxSpeedMagnitude)
            toSet = toSet.normalized * maxSpeedMagnitude;

        physics.velocity = toSet;
    }

    public void TakeControl(float time)
    {
        StartCoroutine(ControlRoutine(time));
    }
    private IEnumerator ControlRoutine(float time)
    {
        TakeControl();
        yield return new WaitForSeconds(time);
        ReleaseControl();
    }
    public void TakeControl(ArrayList parameters = null)
    {
        externallyControlled = true;
    }

    public void ReleaseControl(ArrayList parameters = null)
    {
        externallyControlled = false;
    }

    private void UpdateModelRotation(Vector3 rotation)
    {
        model.transform.localEulerAngles = rotation;
    }

    private void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }
}
