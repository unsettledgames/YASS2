using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyManager : MonoBehaviour
{
    public float totEnergy;
    public float rechargeSpeed;
    public float consumeSpeed;
    public float rechargeTime;

    private float currEnergy;
    private bool isRecharging;
    private PlayerShipController player;

    // Start is called before the first frame update
    void Start()
    {
        currEnergy = totEnergy;
        isRecharging = false;

        player = GetComponent<PlayerShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsSprinting())
            currEnergy -= Time.deltaTime * consumeSpeed;
        else if (currEnergy < totEnergy)
            currEnergy += Time.deltaTime * rechargeSpeed;
        else
            currEnergy = totEnergy;

        if (currEnergy <= 0)
            StartCoroutine(Recharge());
    }

    private IEnumerator Recharge()
    {
        isRecharging = true;
        yield return new WaitForSeconds(rechargeTime);
        isRecharging = false;
    }

    public float GetCurrEneergy()
    {
        return currEnergy;
    }
    
    public bool HasEnergy()
    {
        return currEnergy > 0 && !isRecharging;
    }
}
