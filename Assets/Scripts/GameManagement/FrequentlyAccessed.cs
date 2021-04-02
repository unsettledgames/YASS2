using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequentlyAccessed : MonoBehaviour
{
    public static FrequentlyAccessed Instance;

    public GameObject cameraObject;
    public Camera cameraComponent;

    public GameObject player;
    public PlayerShipController playerController;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = cameraObject.GetComponent<Camera>();
        playerController = player.GetComponent<PlayerShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
