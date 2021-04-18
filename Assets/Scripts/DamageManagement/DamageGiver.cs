using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public float damage;
    public Vector3 knockbackForce;

    private bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        isPlayer = gameObject.layer == 8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            EnemyHealthManager ehm = other.GetComponent<EnemyHealthManager>();
            
            if (ehm != null)
                ehm.TakeDamage(damage);            
        }
        else
        {
            PlayerHealthManager phm = FrequentlyAccessed.Instance.player.GetComponent<PlayerHealthManager>();
            
            phm.TakeDamage(damage, transform.position, knockbackForce);
        }

        Destroy(this.gameObject);
    }
}
