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
    public Vector3 relativeMousePosition;
    public Vector3 globalMousePosition;

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
        relativeMousePosition = Vector3.zero;

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

        shootState = Input.GetButton("Fire1");

        globalMousePosition = Input.mousePosition;
        globalMousePosition.z = 0;

        // Getting mouse position relatively to camera
        relativeMousePosition.x = (Input.mousePosition.x - screenCenter.x) / screenCenter.x;
        relativeMousePosition.y = (Input.mousePosition.y - screenCenter.y) / screenCenter.y;


        DodgeInput();
    }

    private void DodgeInput()
    {
        bool aState = Input.GetKeyDown(KeyCode.A);
        bool dState = Input.GetKeyDown(KeyCode.D);

        // Left dodge
        if (aState && nextATime < Time.time)
        {
            nextATime = Time.time + tapTime;
        }
        else if (aState)
        {
            if (Time.time < nextATime)
            {
                leftDodge = true;
            }
        }

        // Right dodge
        if (dState && nextDTime < Time.time)
        {
            nextDTime = Time.time + tapTime;
        }
        else if (dState)
        {
            if (Time.time < nextDTime)
            {
                rightDodge = true;
            }
        }
    }
}
