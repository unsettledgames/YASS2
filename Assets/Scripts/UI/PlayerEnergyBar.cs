using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyBar : MonoBehaviour
{
    public Image fill;

    private PlayerEnergyManager playerEnergy;
    private float tot;
    private float curr;

    private Vector3 currentScale;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = Vector3.zero;
        currentScale.y = 1;
        currentScale.z = 0;

        playerEnergy = FrequentlyAccessed.Instance.player.GetComponent<PlayerEnergyManager>();

        curr = playerEnergy.GetCurrEneergy();
        tot = playerEnergy.totEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        curr = playerEnergy.GetCurrEneergy();

        currentScale.x = curr / tot;
        fill.transform.localScale = currentScale;
    }
}
