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

        cameraComponent = cameraObject.GetComponent<Camera>();
        playerController = player.GetComponent<PlayerShipController>();
    }
}
