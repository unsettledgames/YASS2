﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
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
        if (canFracture)
            GetComponent<Fracture>().FractureObject();

        if (deathExplosion != null)
            Instantiate(deathExplosion, transform.position, Quaternion.Euler(
                new Vector3(Random.Range(0, 360), 
                Random.Range(0, 360), 
                Random.Range(0, 360)
                ))
        ).GetComponent<Detonator>().Explode();
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