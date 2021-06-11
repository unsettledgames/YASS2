using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public float damage;
    public float knockbackMagnitude;
    public bool destroy = true;

    private bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        isPlayer = gameObject.layer == 8;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            EnemyHealthManager ehm = other.GetComponent<EnemyHealthManager>();
            
            if (ehm != null)
                ehm.TakeDamage(damage);            
        }
        else if (other.tag.Contains("Player"))
        {
            PlayerHealthManager phm = FrequentlyAccessed.Instance.player.GetComponent<PlayerHealthManager>();
            
            if (phm != null)
                phm.TakeDamage(damage, transform.position, knockbackMagnitude);
        }

        if (destroy)
            Destroy(this.gameObject);
    }
}
