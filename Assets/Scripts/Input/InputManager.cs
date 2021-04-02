using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public float mouseZPosition = 10;
    public float tapTime;

    [Header("Input state")]
    public bool shootState;
    public Vector3 mousePosition;

    public float accelerationAmount;
    public float torqueAmount;

    public bool leftDodge;
    public bool rightDodge;

    private float nextATime;
    private float nextDTime;

    public float mouseXAxis;
    public float mouseYAxis;

    private Vector2 screenCenter;

    private void Awake()
    {
        screenCenter = new Vector2();
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;

        Instance = this;

        shootState = false;
        mousePosition = Vector3.zero;

        torqueAmount = 0;
        accelerationAmount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leftDodge = false;
        rightDodge = false;

        accelerationAmount = Input.GetAxis("Acceleration");
        torqueAmount = Input.GetAxis("Torque");

        shootState = Input.GetButtonDown("Fire1");

        // Getting mouse position relatively to camera
        mousePosition.x = (Input.mousePosition.x - screenCenter.x) / screenCenter.x;
        mousePosition.y = (Input.mousePosition.y - screenCenter.y) / screenCenter.y;
        /*
        mousePosition = Input.mousePosition;
        mousePosition.z = mouseZPosition;
        mousePosition = FrequentlyAccessed.Instance.cameraComponent.ScreenToWorldPoint(mousePosition);
        */

        DodgeInput();
    }

    private void DodgeInput()
    {
        bool aState = Input.GetKeyDown(KeyCode.A);
        bool dState = Input.GetKeyDown(KeyCode.D);

        // Left dodge
        if (aState && nextATime < Time.time)
        {
            Debug.Log("Ok A");
            nextATime = Time.time + tapTime;
        }
        else if (aState)
        {
            if (Time.time < nextATime)
            {
                leftDodge = true;
                Debug.Log("Left dodge!");
            }
        }

        // Right dodge
        if (dState && nextDTime < Time.time)
        {
            Debug.Log("Ok D");
            nextDTime = Time.time + tapTime;
        }
        else if (dState)
        {
            if (Time.time < nextDTime)
            {
                rightDodge = true;
                Debug.Log("Right dodge!");
            }
        }
    }
}
