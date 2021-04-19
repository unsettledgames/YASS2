using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public float damage;
    public Vector3 knockbackForce;
    public bool destroy = true;

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
        else if (other.tag.Contains("Player"))
        {
            Debug.Log("Hit " + other.name);
            PlayerHealthManager phm = FrequentlyAccessed.Instance.player.GetComponent<PlayerHealthManager>();
            
            if (phm != null)
                phm.TakeDamage(damage, transform.position, knockbackForce);
        }

        if (destroy)
            Destroy(this.gameObject);
    }
}
