using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public float totHealth = 200;
    
    [Header("Debug, don't set")]
    public float currentHealth;
    private Rigidbody physics;
    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();
        currentHealth = totHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, Vector3 damagePosition, Vector3 knockbackForce)
    {
        Debug.Log("Called");

        currentHealth -= damage;
    }

    public float GetCurrHealth()
    {
        return currentHealth;
    }
}
