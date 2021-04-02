using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    [Tooltip("\"Fractured\" is the object that this will break into")]
    public GameObject fractured;
    public float fractureMagnitude = 5;
    public float fracturedDestroyTime = 5;

    public void FractureObject()
    {
        GameObject fracturedInstance = Instantiate(fractured, transform.position, transform.rotation); //Spawn in the broken version

        fracturedInstance.AddComponent<TimeDestroyer>().time = fracturedDestroyTime;
        Destroy(gameObject); //Destroy the object to stop it getting in the way
    }
}
