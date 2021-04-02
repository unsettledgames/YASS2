using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroyer : MonoBehaviour
{
    public float time;
    public bool initOnStart = true;

    [Header("Animations")]
    public bool fadeOpacity;
    public float fadeOpacitySpeed;
    // Start is called before the first frame update
    void Start()
    {
        if (initOnStart)
            StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
