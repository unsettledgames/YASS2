using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image fill;

    private PlayerHealthManager playerHealth;
    private float tot;
    private float curr;

    private Vector3 currentScale;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = Vector3.zero;
        currentScale.y = 1;
        currentScale.z = 0;
        
        playerHealth = FrequentlyAccessed.Instance.player.GetComponent<PlayerHealthManager>();

        curr = playerHealth.GetCurrHealth();
        tot = playerHealth.totHealth;
    }

    // Update is called once per frame
    void Update()
    {
        curr = playerHealth.GetCurrHealth();

        currentScale.x = curr / tot;
        fill.transform.localScale = currentScale;
    }
}
