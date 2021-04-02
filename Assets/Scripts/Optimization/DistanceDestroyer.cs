using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDestroyer : OptimizedMonoBehaviour
{
    public float distanceFromPlayer;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, FrequentlyAccessed.Instance.player.transform.position) > distanceFromPlayer
            && !isQuitting)
        {
            Destroy(this.gameObject);
        }
    }
}
