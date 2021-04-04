using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject toFollow;
    public GameObject actualTarget;
    public float smoothAmount;

    [Header("VFX")]
    public ParticleSystem accelerationFX;

    void Update()
    {
        float xRot = Mathf.LerpAngle(transform.localEulerAngles.x, actualTarget.transform.localEulerAngles.x, smoothAmount * Time.deltaTime);
        float yRot = Mathf.LerpAngle(transform.localEulerAngles.y, actualTarget.transform.localEulerAngles.y, smoothAmount * Time.deltaTime);
        float zRot = Mathf.LerpAngle(transform.localEulerAngles.z, actualTarget.transform.localEulerAngles.z, smoothAmount * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, toFollow.transform.position, smoothAmount * Time.deltaTime);

        transform.localEulerAngles = new Vector3(xRot, yRot, zRot);

        if (InputManager.Instance.accelerationAmount > 0)
        {
            ParticleSystem.EmissionModule emission = accelerationFX.emission;
            ParticleSystem.MinMaxCurve curve = emission.rateOverTime;

            curve.constantMin = 500;
            curve.constantMax = 500;
            emission.rateOverTime = curve;
        }
        else
        {
            ParticleSystem.EmissionModule emission = accelerationFX.emission;
            ParticleSystem.MinMaxCurve curve = emission.rateOverTime;

            curve.constantMin = 0;
            curve.constantMax = 0;
            emission.rateOverTime = curve;
        }
    }
}