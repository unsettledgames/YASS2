using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : OptimizedMonoBehaviour
{
    public float maxHealth;
    [Header("Death explosion")]
    public bool canFracture;
    public GameObject deathExplosion;

    private float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Die()
    {
        if (!isQuitting)
        {
            if (deathExplosion != null)
                Instantiate(deathExplosion, transform.position, Quaternion.Euler(
                    new Vector3(Random.Range(0, 360),
                    Random.Range(0, 360),
                    Random.Range(0, 360)
                    ))
            ).GetComponent<Detonator>().Explode();

            if (canFracture)
                GetComponent<Fracture>().FractureObject();
            else
                Destroy(this.gameObject);
        }
    }

    public void GiveDamage(float damage)
    {
        this.currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    public float GetCurrHealth()
    {
        return currentHealth;
    }
}
