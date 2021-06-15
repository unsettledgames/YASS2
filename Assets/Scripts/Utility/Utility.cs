using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Vector3 GetRandomVector3(float mag)
    {
        Vector3 ret = Vector3.zero;

        ret.x = Random.Range(-1f, 1f);
        ret.y = Random.Range(-1f, 1f);
        ret.z = Random.Range(-1f, 1f);

        return ret.normalized * mag;
    }
}
