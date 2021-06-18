using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targettable : MonoBehaviour
{
    private PlayerShipController player;
    private Camera camera;

    private bool added = false;
    // Start is called before the first frame update
    void Start()
    {
        camera = FrequentlyAccessed.Instance.cameraComponent;
        player = FrequentlyAccessed.Instance.playerController;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(transform.position);

        if (screenPos.x >= 0 && screenPos.y >= 0)
        {
            if (!added)
            {
                player.AddTargettable(this.gameObject);
                added = true;
            }
        }
        else if (added)
        {
            added = false;
            player.RemoveTargettable(this.gameObject);
        }       
    }

    private void OnDestroy()
    {
        player.RemoveTargettable(this.gameObject);
    }
}
