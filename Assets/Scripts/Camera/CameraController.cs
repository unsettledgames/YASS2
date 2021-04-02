using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject toFollow;
    public GameObject actualTarget;
    public float smoothAmount;

    void Update()
    {
        float xRot = Mathf.LerpAngle(transform.localEulerAngles.x, actualTarget.transform.localEulerAngles.x, smoothAmount * Time.deltaTime);
        float yRot = Mathf.LerpAngle(transform.localEulerAngles.y, actualTarget.transform.localEulerAngles.y, smoothAmount * Time.deltaTime);
        float zRot = Mathf.LerpAngle(transform.localEulerAngles.z, actualTarget.transform.localEulerAngles.z, smoothAmount * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, toFollow.transform.position, smoothAmount * Time.deltaTime);

        transform.localEulerAngles = new Vector3(xRot, yRot, zRot);
    }
}