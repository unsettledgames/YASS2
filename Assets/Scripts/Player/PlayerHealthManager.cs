using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public float totHealth = 200;
    
    [Header("Debug, don't set")]
    public float currentHealth;

    private Rigidbody physics;
    private PlayerShipController controller;
    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerShipController>();

        currentHealth = totHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, Vector3 damagePosition, float knockbackMagnitude)
    {
        currentHealth -= damage;

        StartCoroutine(ApplyKnockback(knockbackMagnitude * (transform.position - damagePosition)));
    }

    private IEnumerator ApplyKnockback(Vector3 knockbackForce)
    {
        controller.TakeControl();

        physics.AddForce(knockbackForce);
        physics.AddTorque(knockbackForce * 100);

        yield return new WaitForSeconds(0.5f);

        controller.ReleaseControl();
    }

    public float GetCurrHealth()
    {
        return currentHealth;
    }
}
