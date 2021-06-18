using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("Randomly generate rotation?")]
    public bool randomRotation;
    [Header("Rotation data")]
    public float rotationMagnitude;
    public Vector3 rotation;
    // Start is called before the first frame update
    void Start()
    {
        if (randomRotation)
            rotation = Utility.GetRandomVector3(rotationMagnitude);
        else
            rotation = rotation.normalized * rotationMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles += rotation * Time.deltaTime;
    }
}
