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
    public float autoAimThreshold = 5f;
    public float autoAimTween = 0.5f;

    public GameObject standardBullet;
    public GameObject missile;

    [Header("Technical")]
    public float timeBetweenDodge;

    // Components
    private Rigidbody physics;
    private PlayerHealthManager healthManager;
    private PlayerEnergyManager energyManager;

    // State
    private bool isDodging = false;
    private bool externallyControlled = false;
    private bool isSprinting = false;

    // Shooting stuff
    private float nextShootTime;
    private GameObject currentTarget;
    private GameObject prevTarget;

    // Targettable enemies
    private List<GameObject> targettables;
    

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();
        energyManager = GetComponent<PlayerEnergyManager>();
        healthManager = GetComponent<PlayerHealthManager>();

        targettables = new List<GameObject>();
        nextShootTime = Time.time + shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!externallyControlled)
        {
            DodgeManagement();

            VelocityManagement();
            PointTowardsMouse();

            ShootManagement();
        }

        prevTarget = currentTarget;
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

        if (InputManager.Instance.accelerationAmount > 0 && energyManager.HasEnergy())
        {
            isSprinting = true;
            // Adding acceleration
            toSet = Vector3.Lerp(toSet, toSet.normalized * maxSpeedMagnitude, InputManager.Instance.accelerationAmount);
        }
        else if (InputManager.Instance.accelerationAmount < 0)
        {
            isSprinting = false;
            // Removing acceleration
            toSet = Vector3.Lerp(toSet.normalized * minSpeedMagnitude, toSet, 1 + InputManager.Instance.accelerationAmount);
        }
        else
        {
            isSprinting = false;
        }

        SetVelocity(toSet);
    }

    private void ShootManagement()
    {
        Vector3 mouseWorldPos = FrequentlyAccessed.Instance.cameraComponent.ScreenToWorldPoint(InputManager.Instance.globalMousePosition);
        RaycastHit aimHit;
        bool hitObject = Physics.Raycast(mouseWorldPos, FrequentlyAccessed.Instance.cameraComponent.transform.forward, 
            out aimHit, Mathf.Infinity, LayerMask.GetMask("AutoAim", "Enemies"));

        if (hitObject)
        {
            if (aimHit.collider.gameObject.layer == 12)
                currentTarget = aimHit.collider.transform.parent.gameObject;
            else
                currentTarget = aimHit.collider.gameObject;

            if (prevTarget == null)
            {
                currentTarget.GetComponent<Outline>().OutlineWidth = Consts.enemyOutlineWidth * 5;
            }
        }
        else
        {
            if (prevTarget != null)
            {
                currentTarget.GetComponent<Outline>().OutlineWidth = 0;
            }
            currentTarget = null;
        }

        if (InputManager.Instance.shootState && Time.time >= nextShootTime)
        {
            // Instantiating projectiles
            GameObject instantiated = Instantiate(standardBullet, standardShootSpawns[0].transform.position, Quaternion.Euler(transform.localEulerAngles));
            GameObject instantiated2 = Instantiate(standardBullet, standardShootSpawns[1].transform.position, Quaternion.Euler(transform.localEulerAngles));

            // Autoaim if the player is hovering an enemy
            if (currentTarget != null)
            {
                Rigidbody targetPhysics = currentTarget.GetComponent<Rigidbody>();
                Vector3 velocity = Vector3.zero;

                if (targetPhysics != null)
                    velocity = targetPhysics.velocity.normalized * autoAimTween;

                Vector3 toLookAt = currentTarget.transform.position;

                Debug.Log("Velocity: " + velocity);

                instantiated.transform.LookAt(toLookAt + velocity);
                instantiated2.transform.LookAt(toLookAt + velocity);
            }

            nextShootTime = Time.time + shootRate;
        }
    }
    /*
    private GameObject GetAutoAimEnemy()
    {
        Camera cameraController = FrequentlyAccessed.Instance.cameraComponent;
        GameObject ret = null;

        Vector3 mousePos = InputManager.Instance.globalMousePosition;
        Vector3 inputPos = Input.mousePosition;

        float minDistance = Mathf.Infinity;
        inputPos.z = 0;

        for (int i=0; i<targettables.Count; i++)
        {
            Vector3 objectPos = cameraController.WorldToScreenPoint(targettables[i].transform.position);
            objectPos.z = 0;
            inputPos.z = 0;
            float distance = Vector3.Distance(objectPos, inputPos);

            if (distance < minDistance && distance < autoAimThreshold && 
                Vector3.Distance(transform.position, targettables[i].transform.position) < autoAimDistance)
            {
                minDistance = distance;
                ret = targettables[i];
            }
        }

        if (ret != null)
            Debug.Log("Got: " + ret.name);
        else
            Debug.Log("Null");

        return ret;
    }
    */
    private void PointTowardsMouse()
    {
        Vector3 mousePosition = InputManager.Instance.relativeMousePosition;
        Vector3 rotation = ObjectPooler.Instance.GetVector3();

        // Setting the new rotation 
        transform.rotation *= Quaternion.Euler(
            -mousePosition.y * Time.deltaTime * torqueSpeed,
            mousePosition.x * Time.deltaTime * torqueSpeed,
            Mathf.Lerp(-maxTorque, maxTorque, 1 - (InputManager.Instance.torqueAmount + 1) / 2)
        );

        rotation.x = Mathf.Lerp(0, 30, Mathf.Abs(mousePosition.y)) * -Mathf.Sign(mousePosition.y);
        rotation.y = 0;
        rotation.z = Mathf.Lerp(0, 60, Mathf.Abs(mousePosition.x)) * -Mathf.Sign(mousePosition.x);
        // Setting rotation (only to make it look cooler)
        model.transform.localEulerAngles = rotation;

        ObjectPooler.Instance.EnqueueVector3(rotation);
    }

    public Vector3 GetCurrentTargetPosition()
    {
        if (currentTarget == null)
            return Vector3.zero;
        return FrequentlyAccessed.Instance.cameraComponent.WorldToScreenPoint(currentTarget.transform.position);
    }

    public bool IsSprinting()
    {
        return isSprinting;
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
        physics.constraints = RigidbodyConstraints.None;
        externallyControlled = true;
    }

    public void ReleaseControl(ArrayList parameters = null)
    {
        physics.constraints = RigidbodyConstraints.FreezeRotation;
        externallyControlled = false;
    }

    public void SetExternalVelocity(Vector3 vel)
    {
        if (externallyControlled)
            SetVelocity(vel);
    }

    public void ExternalLookAt(Vector3 toLookAt)
    {
        if (externallyControlled)
            transform.LookAt(toLookAt);
    }

    public void AddTargettable(GameObject toAdd)
    {
        if (!targettables.Contains(toAdd))
            targettables.Add(toAdd);
    }

    public void RemoveTargettable(GameObject toRemove)
    {
        if (targettables.Contains(toRemove))
            targettables.Remove(toRemove);
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
